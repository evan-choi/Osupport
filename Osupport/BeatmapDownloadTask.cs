namespace Osupport
{
    internal class BeatmapDownloadTask
    {
        public string Id { get; }

        public bool IsBeatmapSet { get; }

        public BeatmapDownloadTask(string id, bool beatmapSet)
        {
            Id = id;
            IsBeatmapSet = beatmapSet;
        }
    }
}
