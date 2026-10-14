using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests.RepositoryIntegrationTests
{
    public class CatalogTreeRepositoryAdapterTests
    {
        private TestApp _testApp;
        private readonly ITestOutputHelper _outputHelper;
        public CatalogTreeRepositoryAdapterTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            TestAppBuilder appBuilder = new();
            _testApp = appBuilder.BuildTestApp(new TestOutputLogger(_outputHelper));
            _testApp.Initialize();
        }


        [Fact]
        public async Task GetUnprocessedNode_DoesntReturnProcessed()
        {
            // Arrange
            await ArrangeData();

            // Act
            var repo = _testApp.ProvideCatalogTreeRepository();
            var firstUnprocessedNode = await repo.GetUnprocessedNodeAsync();
            var sameFirstUnprocessedNode = await repo.GetUnprocessedNodeAsync();

            // Assert
            Assert.Equal(firstUnprocessedNode.DefaultLocation.Url,
                sameFirstUnprocessedNode.DefaultLocation.Url);

            // Act
            // Make it so that would seem that the node's been processed
            firstUnprocessedNode.NodeType = CitilinkNodeType.MerchCatalog;
            firstUnprocessedNode.ScrapeTargetsProcessed = true;
            // We made it seem so.

            await repo.UpdateNodeAsync(firstUnprocessedNode);
            var secondUnprocessedNode = await repo.GetUnprocessedNodeAsync();

            // Assert
            Assert.NotEqual(secondUnprocessedNode.DefaultLocation.Url,
                firstUnprocessedNode.DefaultLocation.Url);
        }

        [Fact]
        public async Task GetUnprocessedNode_IgnoresNodesWithTooMuchProcessingErrors()
        {
            // Arrange
            await ArrangeData();

            // Act
            var repo = _testApp.ProvideCatalogTreeRepository();
            var firstUnprocessedNode = await repo.GetUnprocessedNodeAsync(4);
            Assert.NotNull(firstUnprocessedNode);

            firstUnprocessedNode.ProcessingErrorCount = 30;
            await repo.UpdateNodeAsync(firstUnprocessedNode);
            var unprocessedNode = await repo.GetUnprocessedNodeAsync(4);
            Assert.NotNull(unprocessedNode);
            Assert.True(unprocessedNode.ProcessingErrorCount <= 4);

            // Assert
            Assert.NotEqual(firstUnprocessedNode.DefaultLocation.Url,
                unprocessedNode.DefaultLocation.Url);
        }

        [Fact]
        public async Task AppendBranches_ThrowsOnRepetitiveUrls()
        {
            // Arrange
            await ArrangeData();

            // Act
            var repo = _testApp.ProvideCatalogTreeRepository();
            var unprocessedNode = await repo.GetUnprocessedNodeAsync();
            if (unprocessedNode == null)
                throw new InvalidOperationException();

            CitilinkScrapeTarget firstTarget = new("OneMoreSTUrl");
            CitilinkScrapeTarget secondTarget = new("OneMoreSTUrl");

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            await repo.AppendBranchesAsync(unprocessedNode, [firstTarget, secondTarget]));
        }

        [Fact]
        public async Task BranchesAppended_GlobalDuplicateUrlsIgnored()
        {
            // Arrange
            await ArrangeData();

            // Act
            var repo = _testApp.ProvideCatalogTreeRepository();
            var unprocessedNode = await repo.GetUnprocessedNodeAsync();
            if (unprocessedNode == null)
                throw new InvalidOperationException();

            CitilinkScrapeTarget firstTarget = new("OneMoreSTUrl");
            CitilinkScrapeTarget secondTarget = new("SecondMoreSTUrl");

            await repo.UpdateNodeAsync(unprocessedNode);
            await repo.AppendBranchesAsync(unprocessedNode, [firstTarget, secondTarget]);

            var oneMoreUnprocessedNode = await repo.GetUnprocessedNodeAsync();
            if (oneMoreUnprocessedNode == null)
                throw new InvalidOperationException();
            await repo.AppendBranchesAsync(oneMoreUnprocessedNode, [firstTarget, secondTarget]);

            // Assert
            using var context = _testApp.ProvideDbContextFactory().CreateDbContext();
            var allBranches = context.CitilinkBranches.ToList();
            Assert.Equal(allBranches.DistinctBy(b => b.Url).Count(), allBranches.Count);
        }


        [Fact]
        public async Task BranchesAppended()
        {
            // Arrange
            await ArrangeData();

            // Act
            var repo = _testApp.ProvideCatalogTreeRepository();
            var unprocessedNode = await repo.GetUnprocessedNodeAsync();
            if (unprocessedNode == null)
                throw new InvalidOperationException();

            CitilinkScrapeTarget firstTarget = new("OneMoreSTUrl");
            CitilinkScrapeTarget secondTarget = new("SecondMoreSTUrl");
            await repo.AppendBranchesAsync(unprocessedNode, [firstTarget, secondTarget]);

            // Assert
            using var context = _testApp.ProvideDbContextFactory().CreateDbContext();
            var allBranches = context.CitilinkBranches.ToList();
            var unprocessedBranch = allBranches.Single(b => b.Url == unprocessedNode.DefaultLocation.Url);

            Assert.Equal(1, unprocessedBranch.Children.Count(b => b.Url == firstTarget.Url));
            Assert.Equal(1, unprocessedBranch.Children.Count(b => b.Url == secondTarget.Url));
        }


        [Fact]
        public async Task TestTreeRecreation()
        {
            await ArrangeData();
            var repo = _testApp.ProvideCatalogTreeRepository();
            await repo.RecreateTreeAsync();

            using var context = _testApp.ProvideDbContextFactory().CreateDbContext();
            Assert.Equal(1, context.CitilinkBranches.Count());
            var root = context.CitilinkBranches.Single();
            Assert.Equal("https://www.citilink.ru/catalog/", root.Url);
        }
        protected async Task ArrangeData()
        {
            using var context = await _testApp.ProvideDbContextFactory().CreateDbContextAsync();

            // Шаг 1: очистить остатки предыдущих данных.
            await context.CitilinkParsingExecutionStateEntity.ExecuteDeleteAsync();
            await context.CatalogUrlsTreeEntity.ExecuteDeleteAsync();
            await context.CitilinkBranches.ExecuteDeleteAsync();

            // Шаг 2: подготовить исходные данные.
            CitilinkCatalogBranchEntity root = new("SomeRootUrl", false,
                CitilinkNodeType.RootCatalog);
            CitilinkCatalogUrlsTreeEntity treeEntity = new(root);
            CitilinkParsingExecutionStateEntity newExecutionState = new(false, treeEntity);
            CitilinkCatalogBranchEntity firstBranch = new("someFirstBranchesUrl", false,
                CitilinkNodeType.Unknown);
            CitilinkCatalogBranchEntity secondBranch = new("someSecondBranchesUrl", false,
                CitilinkNodeType.Unknown);
            root.Children.Add(firstBranch);
            firstBranch.Children.Add(secondBranch);
            context.CitilinkParsingExecutionStateEntity.Add(newExecutionState);

            await context.SaveChangesAsync();
        }
    }
}
