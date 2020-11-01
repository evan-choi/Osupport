using System;

namespace Osupport
{
    public sealed class BeatmapNotFoundException : Exception
    {
        public BeatmapNotFoundException(string id) : base($"{id} beatmap not found.")
        {
        }
    }
}
