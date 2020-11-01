using System;
using System.Linq;
using System.Threading.Tasks;
using Osupport.Beatmap.Net;

namespace Osupport.Beatmap.Providers.BloodCat
{
    // https://bloodcat.com/osu/info.php
    public sealed class BloodCatBeatmapProvider : IBeatmapProvider
    {
        private readonly RestClient _client;

        public BloodCatBeatmapProvider()
        {
            _client = new RestClient
            {
                BaseAddress = "http://bloodcat.com"
            };
        }

        public Task<IBeatmapInfo> LookupByIdAsync(string beatmapId)
        {
            return LookupAsync(beatmapId, BloodCatCategory.Beatmap);
        }

        public Task<IBeatmapInfo> LookupBySetIdAsync(string beatmapSetId)
        {
            return LookupAsync(beatmapSetId, BloodCatCategory.BeatmapSet);
        }

        private async Task<IBeatmapInfo> LookupAsync(string q, BloodCatCategory c)
        {
            var query = new QueryStringBuilder()
                .Add("mod", "json")
                .Add("c", c == BloodCatCategory.Beatmap ? "b" : "s")
                .Add("q", q);

            var beatmapSet = (await _client.GetJsonAsync<BloodCatBeatmapSet[]>("/osu/", query)).FirstOrDefault();

            if (beatmapSet == null)
                return null;

            var info = new BloodCatBeatmapInfo
            {
                SetId = beatmapSet.Id,
                Thumbnail = new Uri($"https://b.ppy.sh/thumb/{beatmapSet.Id}l.jpg"),
                Artist = beatmapSet.Artist,
                Title = beatmapSet.Title,
                Creator = beatmapSet.Creator
            };

            if (c == BloodCatCategory.Beatmap)
            {
                var mapIndex = Array.FindIndex(beatmapSet.Beatmaps, map => map.Id == q);

                if (mapIndex >= 0)
                {
                    var beatmap = beatmapSet.Beatmaps[mapIndex];

                    info.Id = beatmap.Id;
                    info.Bpm = beatmap.Bpm;
                    info.Duration = TimeSpan.FromSeconds(beatmap.Length);
                }
            }

            return info;
        }

        public Task<BeatmapDownloadResult> DownloadAsync(IBeatmapInfo beatmapInfo, BeatmapDownloadOption option = null)
        {
            string url = $"https://bloodcat.com/osu/s/{beatmapInfo.SetId}";
            return BeatmapDownloadManager.DownloadAsync(url, beatmapInfo, option);
        }
    }
}
