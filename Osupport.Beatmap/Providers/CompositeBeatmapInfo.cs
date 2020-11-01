using System;

namespace Osupport.Beatmap.Providers
{
    public class CompositeBeatmapInfo : IBeatmapInfo
    {
        public string Id => Info.Id;

        public string SetId => Info.SetId;

        public Uri Thumbnail => Info.Thumbnail;

        public string Artist => Info.Artist;

        public string Title => Info.Title;

        public string Creator => Info.Creator;

        public TimeSpan Duration => Info.Duration;

        public double Bpm => Info.Bpm;

        internal IBeatmapInfo Info { get; }

        internal IBeatmapProvider Provider { get; }

        public CompositeBeatmapInfo(IBeatmapInfo info, IBeatmapProvider provider)
        {
            Info = info;
            Provider = provider;
        }

        public IBeatmapInfo Unwrap()
        {
            if (Info is CompositeBeatmapInfo compositeBeatmapInfo)
                return compositeBeatmapInfo.Unwrap();

            return Info;
        }
    }
}
