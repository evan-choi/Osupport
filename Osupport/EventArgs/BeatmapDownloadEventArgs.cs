using System;
using Osupport.Beatmap;

namespace Osupport
{
    public class BeatmapDownloadEventArgs : EventArgs
    {
        public IBeatmapInfo BeatmapInfo { get; }

        public BeatmapDownloadEventArgs(IBeatmapInfo beatmapInfo)
        {
            BeatmapInfo = beatmapInfo;
        }
    }
}
