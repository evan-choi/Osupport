using System;
using System.Diagnostics;
using System.Linq;
using Osupport.Beatmap;
using Osupport.Beatmap.Providers;
using Osupport.Beatmap.Providers.BloodCat;
using Osupport.Beatmap.Providers.Osu;

namespace Osupport.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var osuProcess = Process.GetProcessesByName("osu!").First();

            var provider = new CompositeProvider(
                new BloodCatBeatmapProvider(),
                new OsuBeatmapProvider("86a40fb966ffaaf1ed3f996d3b50bb0f2a027661")
            );

            using var service = new BeatmapDownloadService(osuProcess, provider);
            service.DownloadProgressChanged += ServiceOnDownloadProgressChanged;
            service.Start();

            Console.ReadLine();
        }

        private static void ServiceOnDownloadProgressChanged(object sender, BeatmapDownloadProgressChangedEventArgs e)
        {
            var beatmapInfo = e.BeatmapInfo;

            if (beatmapInfo is CompositeBeatmapInfo compositeBeatmapInfo)
                beatmapInfo = compositeBeatmapInfo.Unwrap();

            string provider = beatmapInfo is BloodCatBeatmapInfo ? "BloodCat" : "osu!";

            Console.WriteLine($"[{provider}] {e.BeatmapInfo.Title} download {e.Progress:P1}");
        }
    }
}
