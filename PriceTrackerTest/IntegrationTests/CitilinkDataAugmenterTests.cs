using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Core.Utils.DependencyInjection;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTrackerTest.Utils.Logging;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests
{
    public class CitilinkDataAugmenterTests
    {
        private readonly ITestOutputHelper _outputHelper;
        public CitilinkDataAugmenterTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Theory]
        [InlineData("https://www.citilink.ru/catalog/chehly-dlya-smartfonov/", 0)]
        [InlineData("https://www.citilink.ru/catalog/chehly-dlya-smartfonov/", 500)]
        [InlineData("https://www.citilink.ru/catalog/kompyuternye-stoly/", 2500)]
        public async Task Augmenter_AugmentsMerches(string locationUrl,
            int minAugmentMsDelay)
        {
            _outputHelper.WriteLine("Starting test...");
            ServiceCollection collection = new();

            CitilinkUpsertionOptions upsertionOptions = new()
            {
                CitilinkUrl = "https://www.citilink.ru/"
            };

            IOptions<CitilinkUpsertionOptions> interfacedOptions =
                Options.Create<CitilinkUpsertionOptions>(upsertionOptions);

            collection.AddSingleton(interfacedOptions);

            collection.AddSingleton<ILogger>(new TestOutputLogger(_outputHelper));

            Mock<ICitilinkMerchRepositoryFacade> mockMerchRepositoryFacade = new();
            mockMerchRepositoryFacade.Setup(r => r.UpdateManyAsync
            (It.IsAny<List<CitilinkMerchDto>>())).Callback<List<CitilinkMerchDto>>
            (async list =>
            {
                _outputHelper.WriteLine("Merch UpdateMany List starts ---");
                foreach(var dto in list)
                {
                    _outputHelper.WriteLine($"{dto.Name}, {dto.CitilinkId}, " +
                        $"{dto.PriceTrack.CurrentPrice}");
                }
                _outputHelper.WriteLine("Merch UpdateMany List ends ---");
            });

            mockMerchRepositoryFacade.Setup(r => r.CreateManyAsync
            (It.IsAny<List<CitilinkMerchDto>>())).Callback<List<CitilinkMerchDto>>
            (async list =>
            {
                _outputHelper.WriteLine("Merch CreateMany List starts ---");
                foreach (var dto in list)
                {
                    _outputHelper.WriteLine($"{dto.Name}, {dto.CitilinkId}, " +
                        $"{dto.PriceTrack.CurrentPrice}");
                }
                _outputHelper.WriteLine("Merch CreateMany List ends ---");
            });

            mockMerchRepositoryFacade.Setup(r => r.GetMultiple(It.IsAny<string[]>()))
                .Returns([]);

            var shopRepositoryMock = new Mock<IShopRepositoryFacade>();


            // Это успешный сетап мока? сомневаюсь хз посмотрим.
            shopRepositoryMock.Setup(r => r.SingleOrDefault(It.IsAny<Func<ShopDto, bool>>()))
                .Returns(new ShopDto(1, "Citilink", []));


            var catalogTreeRepositoryMock = new Mock<ICitilinkCatalogTreeRepositoryAdapter>();

            catalogTreeRepositoryMock.Setup(r => r.UpdateNodeAsync(It.IsAny<CitilinkNode>()));
            catalogTreeRepositoryMock.Setup(r => r.AppendBranchesAsync(It.IsAny<CitilinkNode>(),
                It.IsAny<List<CitilinkScrapeTarget>>()));

            var merchRepositoryPseudo = mockMerchRepositoryFacade.Object;

            var catalogTreeRepoPseudo = catalogTreeRepositoryMock.Object;

            var shopRepositoryPseudo = shopRepositoryMock.Object;

            collection.AddSingleton<ICitilinkMerchRepositoryFacade>(merchRepositoryPseudo);
            collection.AddSingleton<ICitilinkCatalogTreeRepositoryAdapter>(catalogTreeRepoPseudo);
            collection.AddSingleton<IShopRepositoryFacade>(shopRepositoryPseudo);

            DependencyInjector.InjectMerchDataMaintenanceDependencies(collection);
            var provider = collection.BuildServiceProvider();

            /*
            var merchFacade = provider.GetService<ICitilinkMerchRepositoryFacade>();
            CitilinkMerchDto testDto = new(1243, "Sologoag", new(1, new(), 
                new(1, 14, new(), 125), 123), "assg", 0, 4);
            await merchFacade.CreateManyAsync([testDto]);
            */



            var augmenter = provider.GetService<ICitilinkDataAugmenter>();



            CitilinkEnforcersSharedContext sharedContext = new();
            CitilinkPolicyEnforcersContextfulProvider contextfulProvider = 
                new(sharedContext);

            CitilinkScrapeTarget defaultLocation = new(locationUrl);
            CitilinkNode node = new(defaultLocation, CitilinkNodeAugmentationState.Unprocessed);

            CitilinkExecutionMetadata metaData;

            CitilinkNodeAugmenterAugmentationState augmenterAugmentationState;
            CitilinkNodeAugmentationState nodeAugmentationState;
            CitilinkMerchNodeScrapeTargetsAugmentationState merchNodeAugmentationState;
            int i = 0;

            do
            {
                metaData = await augmenter.AugmentNodeDataAsync(node, contextfulProvider.ProvideForEntireNodeAugmentationEpisode());

                augmenterAugmentationState = metaData.DataSyncMetadata.State;
                nodeAugmentationState = node.AugmentationState;
                merchNodeAugmentationState = node.MerchNodeScrapeTargetsAugmentationState;

                _outputHelper.WriteLine($"{++i} {augmenterAugmentationState} {nodeAugmentationState}\n" +
                    $" {merchNodeAugmentationState}");
                await Task.Delay(minAugmentMsDelay);
            }
            while
            (metaData.DataSyncMetadata.State == CitilinkNodeAugmenterAugmentationState.Success
            && node.AugmentationState != CitilinkNodeAugmentationState.Processed);
        }
    }
}
