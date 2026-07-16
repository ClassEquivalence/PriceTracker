using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;

namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public interface ICitilinkCatalogTreeRepositoryAdapter
    {
        Task RecreateTreeAsync();
        Task<CitilinkNode> GetRootAsync();
        Task AppendBranchesAsync(CitilinkNode parent, List<CitilinkScrapeTarget>
            descendants);
        /// <summary>
        /// Получить любой необработанный узел.
        /// Если возвращается null - необработанные узлы закончились, есть только обработанные.
        /// </summary>
        /// <returns></returns>
        Task<CitilinkNode?> GetUnprocessedNodeAsync(int maxErrorsCount = -1);
        Task UpdateNodeAsync(CitilinkNode node);
    }
}
