using Osupport.Beatmap;

namespace Osupport
{
    public sealed class BeatmapDownloadProgressChangedEventArgs : BeatmapDownloadEventArgs
    {
        public double Progress { get; internal set; }

        public BeatmapDownloadProgressChangedEventArgs(IBeatmapInfo beatmapInfo) : base(beatmapInfo)
        {
        }
    }
}
