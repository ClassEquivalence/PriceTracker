using Moq;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Upsertion;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models;

namespace PriceTrackerTest.UnitTests.Modules.MerchDataMaintenance.DataSynchronizationLayer
{
    public class InnerCitilinkDataAugmenterTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(11)]
        public async Task AugmentNodeAsync_EnqueueCallCountMatchesMerchCount(int merchDtoCount)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(merchDtoCount, 1);

            var merchUpserterMock = new Mock<IInnerCitilinkMerchUpserter>();
            merchUpserterMock.Setup(mu => mu.EnqueueForUpsertionAsync(It.IsAny<CitilinkMerchParsingDto>()));
            var catalogTreeRepositoryMock = new Mock<ICitilinkCatalogTreeRepositoryAdapter>();
            catalogTreeRepositoryMock.Setup(r => r.AppendBranchesAsync(It.IsAny<CitilinkNode>(),
                It.IsAny<List<CitilinkScrapeTarget>>()));
            IInnerCitilinkMerchUpserter merchUpserter = merchUpserterMock.Object;
            ICitilinkCatalogTreeRepositoryAdapter catalogTreeRepositoryAdapter =
                catalogTreeRepositoryMock.Object;
            CitilinkInnerDataAugmenter augmenter = new(merchUpserter, catalogTreeRepositoryAdapter);
            CitilinkNode? node = new CitilinkNode(new(""), CitilinkNodeAugmentationState.Unprocessed);

            CitilinkMerchParsingDto dto = new(2, "22", "22221a");
            List<CitilinkMerchParsingDto> merchesList = new() { dto };
            for (int i = 1; i < merchDtoCount; i++)
            {
                merchesList.Add(dto with { });
            }


            CitilinkParsingSnapshot snapshot = new CitilinkParsingSnapshot()
            {
                DescendantNodeUrls = [],
                Type = ParsableType.CatalogsCatalog,
                Merches = merchesList
            };

            await augmenter.AugmentNodeAsync(node, snapshot);

            merchUpserterMock.Verify(mu => mu.EnqueueForUpsertionAsync(It.IsAny<CitilinkMerchParsingDto>()),
                Times.Exactly(merchDtoCount));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(11)]
        public async Task AugmentNodeAsync_UrlCountMatches(int expectedTargetCount)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(expectedTargetCount, 0);


            int actualTargetCount = 0;
            var merchUpserterMock = new Mock<IInnerCitilinkMerchUpserter>();
            merchUpserterMock.Setup(mu => mu.EnqueueForUpsertionAsync(It.IsAny<CitilinkMerchParsingDto>()));

            var catalogTreeRepositoryMock = new Mock<ICitilinkCatalogTreeRepositoryAdapter>();
            catalogTreeRepositoryMock.Setup(r => r.AppendBranchesAsync(It.IsAny<CitilinkNode>(),
                It.IsAny<List<CitilinkScrapeTarget>>()))
                .Callback((CitilinkNode node, List<CitilinkScrapeTarget> targets) =>
                {
                    actualTargetCount = targets.Count;
                });

            IInnerCitilinkMerchUpserter merchUpserter = merchUpserterMock.Object;
            ICitilinkCatalogTreeRepositoryAdapter catalogTreeRepositoryAdapter =
                catalogTreeRepositoryMock.Object;
            CitilinkInnerDataAugmenter augmenter = new(merchUpserter, catalogTreeRepositoryAdapter);
            CitilinkNode? node = new CitilinkNode(new(""), CitilinkNodeAugmentationState.Unprocessed);

            List<string> descendantsList = [];
            for (int i = 0; i < expectedTargetCount; i++)
            {
                descendantsList.Add($"url{i}");
            }

            CitilinkParsingSnapshot snapshot = new CitilinkParsingSnapshot()
            {
                DescendantNodeUrls = descendantsList,
                Type = ParsableType.CatalogsCatalog,
                Merches = []
            };

            await augmenter.AugmentNodeAsync(node, snapshot);
            Assert.Equal(expectedTargetCount, actualTargetCount);
        }

    }
}
