using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using EasyHook;

namespace Osupport.Hook
{
    public class EntryPoint : IEntryPoint
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool CreateProcessWDelegate(
            [MarshalAs(UnmanagedType.LPTStr)] string lpApplicationName,
            StringBuilder lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            int dwCreationFlags,
            IntPtr lpEnvironment,
            [MarshalAs(UnmanagedType.LPTStr)] string lpCurrentDirectory,
            IntPtr lpStartupInfo,
            IntPtr lpProcessInformation
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateProcess(
            [MarshalAs(UnmanagedType.LPTStr)] string lpApplicationName,
            StringBuilder lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            int dwCreationFlags,
            IntPtr lpEnvironment,
            [MarshalAs(UnmanagedType.LPTStr)] string lpCurrentDirectory,
            IntPtr lpStartupInfo,
            IntPtr lpProcessInformation);

        private LocalHook _hook;
        private readonly HookProxy _proxy;

        public EntryPoint(RemoteHooking.IContext context, string channelName)
        {
            _proxy = RemoteHooking.IpcConnectClient<HookProxy>(channelName);
            _proxy.Ping();
        }

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            var proc = LocalHook.GetProcAddress("kernel32.dll", "CreateProcessW");

            _hook = LocalHook.Create(proc, new CreateProcessWDelegate(CreateProcessWHook), _proxy);
            _hook.ThreadACL.SetExclusiveACL(new int[1]);

            RemoteHooking.WakeUpProcess();

            try
            {
                while (true)
                {
                    Thread.Sleep(500);
                    _proxy.Ping();
                }
            }
            catch
            {
                // ignored
            }
        }

        private static bool CreateProcessWHook(
            string lpApplicationName,
            StringBuilder lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            int dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            IntPtr lpStartupInfo,
            IntPtr lpProcessInformation)
        {
            if (HookRuntimeInfo.Callback is HookProxy proxy &&
                !proxy.CreateProcess(lpApplicationName, lpCommandLine?.ToString()))
            {
                return false;
            }

            return CreateProcess(
                lpApplicationName,
                lpCommandLine,
                lpProcessAttributes,
                lpThreadAttributes,
                bInheritHandles,
                dwCreationFlags,
                lpEnvironment,
                lpCurrentDirectory,
                lpStartupInfo,
                lpProcessInformation);
        }
    }
}
