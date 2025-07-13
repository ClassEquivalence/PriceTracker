using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;
using PriceTracker.Modules.Repository.Facade.Citilink;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class CitilinkMerchDataUpserter :
        IMerchDataConsumer<CitilinkMerchParsingDto>
    {
        private readonly ICitilinkMerchRepositoryFacade _merchRepository;
        private readonly ShopDto _citilinkShop;

        private readonly ILogger? _logger;
        public CitilinkMerchDataUpserter(ICitilinkMerchRepositoryFacade repository, ShopDto
            citilink, ILogger? logger = null)
        {
            _merchRepository = repository;
            _citilinkShop = citilink;

            _logger = logger;
        }
        public async Task Upsert(IAsyncEnumerable<CitilinkMerchParsingDto> complementaryData)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: Upsertion инициирован.");
            // TODO: Хорошо бы вынести логику добавления информации, или раздробить, пересмотреть ответсвтенность
            // за неё.
            await foreach (var parsingDto in complementaryData)
            {
                if (_merchRepository.TryGetSingleByCitilinkId(parsingDto.CitilinkId, out var citilinkMerch)
                    && citilinkMerch != null)
                {
                    _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: товар с CitilinkId = {citilinkMerch.CitilinkId}" +
                        $" в базу уже занесён.");
                    List<TimestampedPriceDto> previousPrices = citilinkMerch.PriceTrack.PreviousTimestampedPricesList.
                        Append(citilinkMerch.PriceTrack.CurrentPrice).ToList();

                    MerchPriceHistoryDto priceHistoryDto = new(default, previousPrices,
                        new(default, parsingDto.Price, DateTime.Now, default), citilinkMerch.Id);

                    CitilinkMerchDto updatedCitilinkMerch = new(default, citilinkMerch.Name, priceHistoryDto,
                        citilinkMerch.CitilinkId, citilinkMerch.ShopId, citilinkMerch.PriceHistoryId);
                    _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: товар с CitilinkId = {citilinkMerch.CitilinkId}" +
                        $" заходит в репозиторий.");
                    _merchRepository.TryUpdate(updatedCitilinkMerch);
                }
                else
                {

                    MerchPriceHistoryDto newPriceHistory = new(default, [], new(default, parsingDto.Price,
                        DateTime.Now, default), default);
                    CitilinkMerchDto newMerch = new CitilinkMerchDto(default, parsingDto.Name, newPriceHistory,
                        parsingDto.CitilinkId, _citilinkShop.Id, default);
                    _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: товара с CitilinkId = {newMerch.CitilinkId}" +
                        $" в базе еще нет. Сейчас он заходит в репозиторий.");
                    _merchRepository.TryInsert(newMerch);
                }
            }
            _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: Upsertion завершился.");
        }
    }
}
