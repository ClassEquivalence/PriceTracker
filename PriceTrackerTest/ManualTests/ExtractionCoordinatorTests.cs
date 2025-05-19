using PriceTracker.Models.DTOModels.ForParsing;
using PriceTracker.Models.Services.MerchDataExtraction;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTrackerTest.ManualTests
{
    public class ExtractionCoordinatorTests
    {
        public MerchExtractionCoordinator Coordinator;
        public List<MerchExtractionAgent> Agents;
        public ExtractionCoordinatorTests()
        {

            Agents = new();
            var agent = new MerchExtractionAgent<CitilinkMerchParsingDto,
                CitilinkMerchExtractionInstruction>()
            Agents.Add(new MerchExtractionAgent());


            Coordinator = new(Agents);
        }

    }
}
