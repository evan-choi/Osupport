using System;
using System.Linq;
using System.Threading.Tasks;
using Osupport.Beatmap.Net;

namespace Osupport.Beatmap.Providers.Osu
{
    // https://github.com/ppy/osu-api/wiki
    public sealed class OsuBeatmapProvider : IBeatmapProvider
    {
        private readonly string _apiKey;
        private readonly RestClient _client;

        public OsuBeatmapProvider(string apiKey)
        {
            _apiKey = apiKey;

            _client = new RestClient
            {
                BaseAddress = "https://osu.ppy.sh/api"
            };
        }

        private QueryStringBuilder CreateQuery()
        {
            return new QueryStringBuilder().Add("k", _apiKey);
        }

        #region Lookup
        public Task<IBeatmapInfo> LookupByIdAsync(string beatmapId)
        {
            return LookupAsync(CreateQuery().Add("b", beatmapId));
        }

        public Task<IBeatmapInfo> LookupBySetIdAsync(string beatmapSetId)
        {
            return LookupAsync(CreateQuery().Add("s", beatmapSetId));
        }

        private async Task<IBeatmapInfo> LookupAsync(QueryStringBuilder query)
        {
            query.Add("limit", 1);

            try
            {
                var info = (await _client.GetJsonAsync<OsuBeatmapInfo[]>("/get_beatmaps", query)).FirstOrDefault();

                if (info?.IsDownloadUnavailable ?? false)
                    return null;

                return info;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public Task<BeatmapDownloadResult> DownloadAsync(IBeatmapInfo beatmapInfo, BeatmapDownloadOption option = null)
        {
            throw new NotSupportedException();
        }
    }
}
