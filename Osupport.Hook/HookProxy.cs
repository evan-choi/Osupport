using System;

namespace Osupport.Hook
{
    public class HookProxy : MarshalByRefObject
    {
        public delegate bool CreateProcessDelegate(string application, string command);

        public event CreateProcessDelegate OnCreateProcess;

        public bool CreateProcess(string application, string command)
        {
            return OnCreateProcess?.Invoke(application, command) ?? true;
        }

        public void Ping()
        {
        }
    }
}
