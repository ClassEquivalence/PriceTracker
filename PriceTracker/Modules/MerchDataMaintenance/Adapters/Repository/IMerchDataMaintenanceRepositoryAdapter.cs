using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;

namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public interface IMerchDataMaintenanceRepositoryAdapter
    {
        /// <summary>
        /// Данный метод не станет сохранять или менять магазин.
        /// </summary>
        /// <param name="citilinkMerches"></param>
        /// <returns></returns>
        Task CreateManyAsync(List<CitilinkMerchDto> citilinkMerches);
        /// <summary>
        /// Данный метод не станет сохранять или менять магазин.
        /// </summary>
        /// <param name="citilinkMerches"></param>
        /// <returns></returns>
        Task UpdateManyAsync(List<CitilinkMerchDto> citilinkMerches);
        List<CitilinkMerchDto> GetMultiple(string[] citilinkIds);
    }
}
