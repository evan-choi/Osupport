using Newtonsoft.Json;

namespace Osupport.Beatmap.Providers.BloodCat
{
    internal sealed class BloodCatBeatmap
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("bpm")]
        public double Bpm { get; set; }
    }
}
