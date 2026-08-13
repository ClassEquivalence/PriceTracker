using Microsoft.Extensions.Logging;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.Repository;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests.RepositoryIntegrationTests
{
    public class CitilinkMiscRepositoryTests
    {
        ICitilinkMiscellaneousRepositoryFacade _repository;
        private readonly TestApp _testApp;
        private readonly ILogger _logger;

        public CitilinkMiscRepositoryTests(ITestOutputHelper outputHelper)
        {
            TestAppBuilder appBuilder = new();
            _testApp = appBuilder.BuildWithDefaults(new TestOutputLogger(outputHelper));

            _testApp.Initialize();

            _repository = _testApp.ProvideMiscRepository();
            _logger = _testApp.ProvideLogger();
        }

        [Fact]
        public async Task TestExtractionStateSave()
        {
            CitilinkBranch branchA = new(default, "BranchA", [], false, CitilinkNodeType.Unknown,
                CitilinkNodeAugmentationState.Unprocessed);
            CitilinkBranch root = new(default, "roooootik", [branchA], false, CitilinkNodeType.RootCatalog,
                CitilinkNodeAugmentationState.PartiallyProcessed_TypeIsKnown);
            CatalogUrlsTree tree = new(root);
            CitilinkExtractionStateDto extractionStateDto = new(tree, false);
            _repository.Save(extractionStateDto);


            extractionStateDto = _repository.Provide();
            branchA = extractionStateDto.CachedUrls.Root.Children.First();
            if (!(branchA != null && branchA.Url == "BranchA"))
                Assert.Fail("BranchA is null or wrong");

            CitilinkBranch branchC = new(default, "BranchC", [], false, CitilinkNodeType.Unknown,
               CitilinkNodeAugmentationState.Unprocessed);
            CitilinkBranch branchB = new(default, "BranchB", [branchC], false, CitilinkNodeType.Unknown,
                CitilinkNodeAugmentationState.Unprocessed);

            branchA.Children.Add(branchB);
            _repository.Save(extractionStateDto);

            extractionStateDto = _repository.Provide();
            branchA = extractionStateDto.CachedUrls.Root.Children.First();
            branchB = branchA.Children.First();
            if (branchB is null)
                Assert.Fail("BranchB is null!");
            _logger?.LogTrace($"BranchB is {branchB}");
            branchC = branchB.Children.First();
            if (!(branchC != null && branchC.Url == "BranchC"))
                Assert.Fail("BranchC is null or wrong");
            _logger?.LogTrace($"BranchC is {branchC}");

        }
    }
}
