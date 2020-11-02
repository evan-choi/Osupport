using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Osupport.Beatmap;
using Osupport.Beatmap.Providers;
using Osupport.Beatmap.Providers.BloodCat;
using Osupport.Beatmap.Providers.Osu;
using Osupport.Test.Resources;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Osupport.Test
{
    internal class Program
    {
        private const string processTarget = "osu!";

        public static void Main(string[] args)
        {
            Console.WriteLine(Resource.GetString("Initialize.txt"));
            Console.WriteLine();

            InitializeLogger();

            var provider = new CompositeProvider(
                new BloodCatBeatmapProvider(),
                new OsuBeatmapProvider("86a40fb966ffaaf1ed3f996d3b50bb0f2a027661")
            );

            while (true)
            {
                var process = FindTargetProcess();

                Log.Information("Found {target} process({pid})", processTarget, process.Id);

                using var service = new BeatmapDownloadService(process, provider);

                service.DownloadStarted += ServiceOnDownloadStarted;
                service.DownloadProgressChanged += ServiceOnDownloadProgressChanged;
                service.DownloadCompleted += ServiceOnDownloadCompleted;
                service.DownloadFailed += ServiceOnDownloadFailed;

                service.Start();

                while (!process.HasExited)
                {
                    Thread.Sleep(500);
                }

                Log.Information("{target} process({pid}) closed", processTarget, process.Id);
                Console.WriteLine();
            }
        }

        private static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }

        private static void ServiceOnDownloadStarted(object sender, BeatmapDownloadEventArgs e)
        {
            Log.Information("[{provider}] {title} - Start download",
                ResolveProviderName(e.BeatmapInfo),
                e.BeatmapInfo.Title);
        }

        private static void ServiceOnDownloadProgressChanged(object sender, BeatmapDownloadProgressChangedEventArgs e)
        {
            Log.Information("[{provider}] {title} - {progress:P1}",
                ResolveProviderName(e.BeatmapInfo),
                e.BeatmapInfo.Title,
                e.Progress);
        }

        private static void ServiceOnDownloadCompleted(object sender, BeatmapDownloadEventArgs e)
        {
            Log.Information("[{provider}] {title} - Done",
                ResolveProviderName(e.BeatmapInfo),
                e.BeatmapInfo.Title);
        }

        private static void ServiceOnDownloadFailed(object sender, BeatmapDownloadFailedEventArgs e)
        {
            if (e.Exception is BeatmapNotFoundException)
            {
                Log.Error(e.Exception.Message);
                return;
            }

            if (e.BeatmapInfo != null)
            {
                Log.Error("[{provider}] {title} - Failed download",
                    ResolveProviderName(e.BeatmapInfo),
                    e.BeatmapInfo.Title);
            }
            else
            {
                Log.Error(e.Exception, "Beatmap download failed.");
            }
        }

        private static Process FindTargetProcess()
        {
            Process process;
            bool first = false;

            while ((process = Process.GetProcessesByName(processTarget).FirstOrDefault()) == null)
            {
                if (!first)
                {
                    Log.Information("Wait {target} process ..", processTarget);
                    first = true;
                }

                Thread.Sleep(1000);
            }

            return process;
        }

        private static string ResolveProviderName(IBeatmapInfo beatmapInfo)
        {
            if (beatmapInfo is CompositeBeatmapInfo compositeBeatmapInfo)
                beatmapInfo = compositeBeatmapInfo.Unwrap();

            return beatmapInfo is BloodCatBeatmapInfo ? "BloodCat" : "osu!";
        }
    }
}
