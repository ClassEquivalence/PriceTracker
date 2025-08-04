using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;

namespace PriceTracker.Modules.Repository.Facade.Citilink
{
    public interface ICitilinkMerchRepositoryFacade
    {
        List<CitilinkMerchDto> Where(Func<CitilinkMerchDto, bool> selector);

        bool TryGetSingleByCitilinkId(string citilinkId, out CitilinkMerchDto? dto);
        bool TryUpdate(CitilinkMerchDto dto);
        bool TryInsert(CitilinkMerchDto dto);

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
