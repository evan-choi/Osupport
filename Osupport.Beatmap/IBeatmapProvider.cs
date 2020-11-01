using System.Threading.Tasks;

namespace Osupport.Beatmap
{
    public interface IBeatmapProvider
    {
        Task<IBeatmapInfo> LookupByIdAsync(string beatmapId);

        Task<IBeatmapInfo> LookupBySetIdAsync(string beatmapSetId);

        Task<BeatmapDownloadResult> DownloadAsync(IBeatmapInfo beatmapInfo, BeatmapDownloadOption option = null);
    }
}
