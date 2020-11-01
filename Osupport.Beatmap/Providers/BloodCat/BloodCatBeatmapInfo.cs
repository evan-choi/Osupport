using System;

namespace Osupport.Beatmap.Providers.BloodCat
{
    public sealed class BloodCatBeatmapInfo : IBeatmapInfo
    {
        public string Id { get; set; }

        public string SetId { get; set; }

        public Uri Thumbnail { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Creator { get; set; }

        public double Bpm { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
