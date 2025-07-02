
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;
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
            /*
            Agents = new();
            var agent = new MerchExtractionAgent<CitilinkMerchParsingDto,
                CitilinkMerchExtractionInstruction>()
            Agents.Add(new MerchExtractionAgent());


            Coordinator = new(Agents);
            */
        }

    }
}
