using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop;
using PriceTracker.Modules.WebInterface.Mapping.Merch;
using PriceTracker.Modules.WebInterface.Mapping.Shop;
using PriceTracker.Modules.WebInterface.Services.MerchService;
using PriceTracker.Modules.WebInterface.Services.MerchService.Citilink;
using PriceTracker.Modules.WebInterface.Services.ShopService;

namespace PriceTracker.Modules.WebInterface.Services.InterfaceServices
{
    public class AdminAPIService
    {
        private readonly IDetailedMerchDtoMapper _detailedMerchMapper;
        private readonly IOverviewMerchDtoMapper _overviewMerchMapper;
        private readonly IShopNameMapper _shopNameMapper;
        private readonly IShopOverviewMapper _shopOverviewMapper;

        private readonly IMerchService _merchService;
        private readonly IShopService _shopService;
        private readonly ICitilinkMerchService _citilinkMerchService;


        public AdminAPIService(IDetailedMerchDtoMapper detailedMerchDtoMapper,
            IOverviewMerchDtoMapper overviewMerchDtoMapper, IMerchService merchService,
            IShopService shopService, IShopNameMapper shopNameMapper,
            IShopOverviewMapper shopOverviewMapper, ICitilinkMerchService
            citilinkMerchService)
        {

            _detailedMerchMapper = detailedMerchDtoMapper;
            _overviewMerchMapper = overviewMerchDtoMapper;
            _shopNameMapper = shopNameMapper;
            _shopOverviewMapper = shopOverviewMapper;

            _merchService = merchService;
            _shopService = shopService;
            _citilinkMerchService = citilinkMerchService;

        }

        public IEnumerable<MerchOverviewDto> GetMerchesOfShop(int shopId)
        {
            var merches = _merchService.GetMerchesOfShop(shopId).
                Select(_overviewMerchMapper.Map);
            return merches;
        }


        public DetailedMerchDto? GetDetailedMerch(int merchId)
        {
            var merchModel = _merchService.GetMerch(merchId);
            return merchModel != null ? _detailedMerchMapper.Map(merchModel) : null;
        }

        public DetailedMerchDto? GetDetailedCitilinkMerch(string citilinkMerchCode)
        {
            var citilinkMerchModel = _citilinkMerchService.GetByCitilinkId(citilinkMerchCode);
            return citilinkMerchModel != null ? _detailedMerchMapper.
                Map(citilinkMerchModel) : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="merch"></param>
        /// <returns>True в случае успешного создания.
        /// False в случае неуспешного.</returns>
        public bool CreateMerch(int shopId, MerchOverviewDto merch)
        {
            var shop = _shopService.GetShopById(shopId);
            if (shop == null)
                return false;

            MerchPriceHistoryDto priceHistoryDto = new(default, [], new(default,
                merch.CurrentPrice, DateTime.Now, default), merch.Id);
            bool isAdded = _merchService.TryCreate(new MerchDto(merch.Id, merch.Name, priceHistoryDto,
                default, default));

            return isAdded;
        }


        public bool ChangeMerchName(int merchId, string name)
        {
            bool isChanged = _merchService.TryChangeName(merchId, name);
            return isChanged;
        }


        public bool DeleteMerch(int merchId)
        {
            if (_merchService.TryDelete(merchId))
                return true;
            else
                return false;
        }



        public bool AddPreviousPrice(int merchId, TimestampedPriceDto timestampedPrice)
        {

            DateTime dateTime = timestampedPrice.DateTime == default ?
                DateTime.Now : timestampedPrice.DateTime;

            bool isAdded = _merchService.TryAddTimestampedPrice(merchId,
                new TimestampedPriceDto(default, timestampedPrice.Price, dateTime,
                default));

            return isAdded;
        }



        public bool SetCurrentPrice(int merchId, decimal currentPrice)
        {
            bool isPriceSet = _merchService.SetCurrentPrice(merchId, currentPrice);
            return isPriceSet;
        }


        public bool RemoveTimestampedPrice(int timestampedPriceId)
        {
            bool isRemoved = _merchService.RemoveSingleTimestampedPrice(timestampedPriceId);
            return isRemoved;
        }


        public bool ClearOldPrices(int merchId)
        {
            bool areRemoved = _merchService.ClearOldPrices(merchId);
            return areRemoved;
        }


        public IEnumerable<ShopNameDto> GetShops()
        {
            return _shopService.Shops.Select(_shopNameMapper.Map);
        }



        public ShopOverviewDto? GetShop(int id)
        {
            var shop = _shopService.GetShopById(id);

            // Линк можно собрать в маппере на основе имеющегося Id. Наверное.
            // Если его точно передать в модель.
            // var link = _linkBuilder.GetShopMerchesPath(id);
            return shop != null ?
                _shopOverviewMapper.Map(shop) : null;
        }



        public bool CreateShop(ShopNameDto shop)
        {
            if (string.IsNullOrWhiteSpace(shop.Name))
            {
                return false;
            }
            ShopDto shopDto = new(default, shop.Name, []);
            bool isAdded = _shopService.AddShop(shopDto);

            return isAdded;
        }



        public bool ChangeShopName(int id, string shopName)
        {
            var shop = _shopService.GetShopById(id);
            bool isNameChanged = shop != null && _shopService.ChangeShopName(shop, shopName);
            return isNameChanged;
        }



        public bool DeleteShop(int id)
        {
            bool isRemoved = _shopService.RemoveShopById(id);
            return isRemoved;
        }


    }
}
