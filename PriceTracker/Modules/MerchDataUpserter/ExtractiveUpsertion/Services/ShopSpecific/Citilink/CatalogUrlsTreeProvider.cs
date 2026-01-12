using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Mapping;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Facade.Citilink;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink
{
    public class CatalogUrlsTreeProvider
    {

        private readonly ICitilinkMiscellaneousRepositoryFacade _repository;
        private readonly CitilinkUpsertionOptions _options;
        private readonly CitilinkCatalogUrlsTreeMapper _mapper;
        private CitilinkCatalogUrlsTree? _cached;

        public CatalogUrlsTreeProvider(ICitilinkMiscellaneousRepositoryFacade repository,
            CitilinkUpsertionOptions options, CitilinkCatalogUrlsTreeMapper mapper)
        {
            _repository = repository;
            _options = options;
            _mapper = mapper;
        }

        public CitilinkCatalogUrlsTree Provide()
        {
            return _cached = _cached ?? TryLoad() ?? Create();
        }

        public CitilinkCatalogUrlsTree Create()
        {
            BranchWithFunctionality root = new(default, _options.CitilinkMainCatalogUrl, []);

            return new CitilinkCatalogUrlsTree(root);
        }
        private CitilinkCatalogUrlsTree? TryLoad()
        {
            var treeBeforeMapped = _repository.Provide()?.CachedUrls;
            return treeBeforeMapped != null ? _mapper.Map(treeBeforeMapped)
                : null;
        }
    }
}
