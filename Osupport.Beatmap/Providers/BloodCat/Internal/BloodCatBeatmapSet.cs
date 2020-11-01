using Newtonsoft.Json;

namespace Osupport.Beatmap.Providers.BloodCat
{
    internal sealed class BloodCatBeatmapSet
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("beatmaps")]
        public BloodCatBeatmap[] Beatmaps { get; set; }
    }
}
