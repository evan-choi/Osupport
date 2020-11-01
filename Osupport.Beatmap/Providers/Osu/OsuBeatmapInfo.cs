using System;
using Newtonsoft.Json;

namespace Osupport.Beatmap.Providers.Osu
{
    public sealed class OsuBeatmapInfo : IBeatmapInfo
    {
        [JsonProperty("beatmap_id")]
        public string Id { get; set; }

        [JsonProperty("beatmapset_id")]
        public string SetId
        {
            get => _setId;
            set
            {
                _setId = value;
                Thumbnail = new Uri($"https://b.ppy.sh/thumb/{SetId}l.jpg");
            }
        }

        [JsonIgnore]
        public Uri Thumbnail { get; private set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("hit_length")]
        public int HitLength
        {
            get => _hitLength;
            set
            {
                _hitLength = value;
                Duration = TimeSpan.FromSeconds(value);
            }
        }

        [JsonIgnore]
        public TimeSpan Duration { get; private set; }

        [JsonProperty("bpm")]
        public double Bpm { get; set; }

        [JsonProperty("download_unavailable")]
        public int DownloadUnavailable { get; set; }

        [JsonIgnore]
        public bool IsDownloadUnavailable => DownloadUnavailable == 1;

        #region Fields
        private string _setId;
        private int _hitLength;
        #endregion
    }
}
