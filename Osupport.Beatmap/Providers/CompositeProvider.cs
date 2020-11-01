using System;
using System.Threading.Tasks;

namespace Osupport.Beatmap.Providers
{
    public class CompositeProvider : IBeatmapProvider
    {
        private readonly IBeatmapProvider[] _providers;

        public CompositeProvider(params IBeatmapProvider[] providers)
        {
            _providers = providers;
        }

        public async Task<IBeatmapInfo> LookupByIdAsync(string beatmapId)
        {
            foreach (var provider in _providers)
            {
                try
                {
                    var info = await provider.LookupByIdAsync(beatmapId);

                    if (info == null)
                        continue;

                    return new CompositeBeatmapInfo(info, provider);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        public async Task<IBeatmapInfo> LookupBySetIdAsync(string beatmapSetId)
        {
            foreach (var provider in _providers)
            {
                try
                {
                    var info = await provider.LookupBySetIdAsync(beatmapSetId);

                    if (info == null)
                        continue;

                    return new CompositeBeatmapInfo(info, provider);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        public Task<BeatmapDownloadResult> DownloadAsync(IBeatmapInfo beatmapInfo, BeatmapDownloadOption option = null)
        {
            var compositeBeatmapInfo = (CompositeBeatmapInfo)beatmapInfo;
            return compositeBeatmapInfo.Provider.DownloadAsync(compositeBeatmapInfo.Info, option);
        }
    }
}
