using System;
using Osupport.Beatmap;

namespace Osupport
{
    public sealed class BeatmapDownloadFailedEventArgs : BeatmapDownloadEventArgs
    {
        public Exception Exception { get; }

        public BeatmapDownloadFailedEventArgs(IBeatmapInfo beatmapInfo, Exception exception) : base(beatmapInfo)
        {
            Exception = exception;
        }
    }
}
