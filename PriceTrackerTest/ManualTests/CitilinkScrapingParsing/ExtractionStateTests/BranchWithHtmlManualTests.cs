using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTrackerTest.Utils.CustomAttributes;
using PriceTrackerTest.Utils.Logging.LoggerProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PriceTrackerTest.ManualTests.CitilinkScrapingParsing.ExtractionStateTests
{
    public class BranchWithHtmlManualTests
    {

        private readonly ITestOutputHelper _output;


        public BranchWithHtmlManualTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [ManualFact]
        public void RemoveFiltersAndDuplicates_ManualCheck()
        {
            var url = $"https://www.citilink.ru/catalog/printery-lazernye--lazernye-mfu-mainmenu/?ref=mainmenu_left";
            BranchWithHtml branch = new(default, url, []);
            _output.WriteLine(branch.GetCategoryString());
            _output.WriteLine(branch.GetCategorySlug());
            branch.RemoveFilter();
            _output.WriteLine(branch.Url);
        }
    }
}
