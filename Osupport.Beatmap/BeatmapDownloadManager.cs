using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Osupport.Beatmap
{
    internal static class BeatmapDownloadManager
    {
        public static async Task<BeatmapDownloadResult> DownloadAsync(string url, IBeatmapInfo beatmapInfo, BeatmapDownloadOption option = null)
        {
            var filePath = Path.GetFullPath(option?.Path ?? $"{beatmapInfo.SetId}.osz");

            if (File.Exists(filePath))
                return new BeatmapDownloadResult(option, filePath);

            using var client = new WebClient();

            if (option?.Progress != null)
            {
                client.DownloadProgressChanged += (sender, args) =>
                {
                    option.Progress.Report(args.ProgressPercentage / 100d);
                };
            }

            try
            {
                await client.DownloadFileTaskAsync(url, filePath);
                return new BeatmapDownloadResult(option, filePath);
            }
            catch (Exception e)
            {
                return new BeatmapDownloadResult(option, e);
            }
        }
    }
}
