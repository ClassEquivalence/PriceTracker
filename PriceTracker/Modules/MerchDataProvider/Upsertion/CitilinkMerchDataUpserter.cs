using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;
using PriceTracker.Modules.MerchDataProvider.Upsert;
using PriceTracker.Modules.Repository.Facade;

namespace PriceTracker.Modules.MerchDataProvider.Upsertion
{
    public class CitilinkMerchDataUpserter : MerchDataUpserter<CitilinkMerchDto, CitilinkMerchParsingDto>,
        IMerchDataConsumer<CitilinkMerchParsingDto>
    {
        private readonly ICitilinkMerchRepositoryFacade _merchRepository;
        private readonly ShopDto _citilinkShop;
        public CitilinkMerchDataUpserter(ICitilinkMerchRepositoryFacade repository, ShopDto
            citilink)
        {
            _merchRepository = repository;
            _citilinkShop = citilink;
        }
        public async Task ReceiveAsync(IAsyncEnumerable<CitilinkMerchParsingDto> parsingDtos)
        {
            await Upsert(parsingDtos);
        }

        protected async override Task Upsert(IAsyncEnumerable<CitilinkMerchParsingDto> complementaryData)
        {
            // TODO: Хорошо бы вынести логику добавления информации, или раздробить, пересмотреть ответсвтенность
            // за неё.
            await foreach (var parsingDto in complementaryData)
            {
                if (_merchRepository.TryGetSingleByCitilinkId(parsingDto.CitilinkId, out var citilinkMerch)
                    && citilinkMerch != null)
                {
                    List<TimestampedPriceDto> previousPrices = citilinkMerch.PriceTrack.PreviousTimestampedPricesList.
                        Append(citilinkMerch.PriceTrack.CurrentPrice).ToList();

                    MerchPriceHistoryDto priceHistoryDto = new(previousPrices, new(parsingDto.Price, DateTime.Now));

                    CitilinkMerchDto updatedCitilinkMerch = new(citilinkMerch.Name, priceHistoryDto,
                        citilinkMerch.Shop, citilinkMerch.CitilinkId);
                    _merchRepository.TryUpdate(updatedCitilinkMerch);
                }
                else
                {
                    _merchRepository.TryInsert(new(parsingDto.Name, new([], new(parsingDto.Price, DateTime.Now)),
                        _citilinkShop, parsingDto.CitilinkId));
                }

            }
        }
    }
}
