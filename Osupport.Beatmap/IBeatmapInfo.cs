using System;

namespace Osupport.Beatmap
{
    public interface IBeatmapInfo
    {
        string Id { get; }

        string SetId { get; }

        Uri Thumbnail { get; }

        string Artist { get; }

        string Title { get; }

        string Creator { get; }

        double Bpm { get; }

        TimeSpan Duration { get; }
    }
}
