using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EasyHook;
using Osupport.Beatmap;
using Osupport.Hook;

namespace Osupport
{
    public sealed class BeatmapDownloadService : IDisposable
    {
        private const string hookDll = "Osupport.Hook.dll";

        public event EventHandler<BeatmapDownloadEventArgs> DownloadStarted;

        public event EventHandler<BeatmapDownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<BeatmapDownloadEventArgs> DownloadCompleted;

        public event EventHandler<BeatmapDownloadFailedEventArgs> DownloadFailed;

        private HookProxy _hookProxy;

        private readonly Process _process;
        private readonly IBeatmapProvider _beatmapProvider;
        private readonly Regex _beatmapUrlPattern;
        private readonly SynchronizationContext _synchronizationContext;

        private Thread _downloadThread;
        private BlockingCollection<BeatmapDownloadTask> _downloadTasks;

        public BeatmapDownloadService(Process process, IBeatmapProvider beatmapProvider)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
            _beatmapProvider = beatmapProvider ?? throw new ArgumentNullException(nameof(beatmapProvider));

            _beatmapUrlPattern = new Regex(
                @"https?:\/\/osu\.ppy\.sh\/(?<type>[sb])\/(?<id>\d+)\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            _synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        public void Start()
        {
            if (_hookProxy != null)
                return;

            _hookProxy = new HookProxy();
            _hookProxy.OnCreateProcess += OnOnCreateProcess;

            string channelName = null;

            RemoteHooking.IpcCreateServer(ref channelName, WellKnownObjectMode.SingleCall, _hookProxy);
            RemoteHooking.Inject(_process.Id, hookDll, hookDll, channelName);

            _downloadThread = new Thread(DownloadProc);
            _downloadTasks = new BlockingCollection<BeatmapDownloadTask>();

            _downloadThread.Start();
        }

        private void DownloadProc()
        {
            foreach (var task in _downloadTasks.GetConsumingEnumerable())
            {
                IBeatmapInfo beatmapInfo = null;

                try
                {
                    Task<IBeatmapInfo> infoTask = task.IsBeatmapSet ?
                        _beatmapProvider.LookupBySetIdAsync(task.Id) :
                        _beatmapProvider.LookupByIdAsync(task.Id);

                    beatmapInfo = infoTask.Result ?? throw new BeatmapNotFoundException(task.Id);

                    DownloadStarted?.Invoke(this, new BeatmapDownloadEventArgs(beatmapInfo));

                    var option = new BeatmapDownloadOption();

                    if (DownloadProgressChanged != null)
                        option.Progress = new PropagateHandler(this, beatmapInfo);

                    var result = _beatmapProvider.DownloadAsync(beatmapInfo, option).Result;

                    if (result.Exception != null)
                        throw result.Exception;

                    DownloadCompleted?.Invoke(this, new BeatmapDownloadEventArgs(beatmapInfo));

                    if (File.Exists(_process.MainModule?.FileName))
                    {
                        Process.Start(_process.MainModule.FileName, result.FilePath);
                    }
                }
                catch (Exception e)
                {
                    DownloadFailed?.Invoke(this, new BeatmapDownloadFailedEventArgs(beatmapInfo, e));

                    var fallbackUrl = task.IsBeatmapSet ?
                        $"https://osu.ppy.sh/beatmapsets/{task.Id}" :
                        $"https://osu.ppy.sh/b/{task.Id}";

                    Process.Start(fallbackUrl);
                }
            }
        }

        private bool OnOnCreateProcess(string application, string command)
        {
            if (string.IsNullOrEmpty(command))
                return true;

            var match = _beatmapUrlPattern.Match(command);

            if (!match.Success)
                return true;

            string type = match.Groups["type"].Value.ToLower();
            string id = match.Groups["id"].Value;

            _downloadTasks.Add(new BeatmapDownloadTask(id, type == "s"));

            return false;
        }

        public void Dispose()
        {
            _downloadTasks.CompleteAdding();
            _downloadTasks?.Dispose();
        }

        private sealed class PropagateHandler : IProgress<double>
        {
            private readonly BeatmapDownloadProgressChangedEventArgs _eventArgs;
            private readonly BeatmapDownloadService _service;

            public PropagateHandler(BeatmapDownloadService service, IBeatmapInfo beatmapInfo)
            {
                _service = service;
                _eventArgs = new BeatmapDownloadProgressChangedEventArgs(beatmapInfo);

                Report(0);
            }

            public void Report(double value)
            {
                _service._synchronizationContext.Post(ReportSync, value);
            }

            private void ReportSync(object state)
            {
                _eventArgs.Progress = (double)state;
                _service.DownloadProgressChanged.Invoke(_service, _eventArgs);
            }
        }
    }
}
