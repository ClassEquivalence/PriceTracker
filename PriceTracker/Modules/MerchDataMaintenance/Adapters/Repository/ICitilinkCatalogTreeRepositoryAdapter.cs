using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public interface ICitilinkCatalogTreeRepositoryAdapter
    {
        Task RecreateTreeAsync();
        Task AppendBranchesAsync(CitilinkNode parent, List<CitilinkScrapeTarget>
            descendants);
        /// <summary>
        /// Получить любой необработанный узел.
        /// Если возвращается null - необработанные узлы закончились, есть только обработанные.
        /// </summary>
        /// <returns></returns>
        Task<CitilinkNode?> GetUnprocessedNodeAsync(int maxErrorsCount = -1);

        /// <summary>
        /// Работает ТОЛЬКО с ненавигационными, собственными полями узла.
        /// Для работы со связанными сущностями допускается использование других методов.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Task UpdateNodeAsync(CitilinkNode node);
    }
}
