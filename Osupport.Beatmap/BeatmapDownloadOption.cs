using System;

namespace Osupport.Beatmap
{
    public sealed class BeatmapDownloadOption
    {
        public string Path { get; set; }

        public IProgress<double> Progress { get; set; }
    }
}