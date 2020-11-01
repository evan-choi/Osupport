using System;

namespace Osupport.Beatmap
{
    public sealed class BeatmapDownloadResult
    {
        public BeatmapDownloadOption Option { get; }

        public string FilePath { get; }

        public Exception Exception { get; }

        public bool IsSuccess => Exception == null;

        public BeatmapDownloadResult(BeatmapDownloadOption option, string filePath)
        {
            Option = option;
            FilePath = filePath;
        }

        public BeatmapDownloadResult(BeatmapDownloadOption option, Exception exception)
        {
            Option = option;
            Exception = exception;
        }
    }
}
