using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PriceTracker.Models.Services.ScrapingServices.ShopSpecificModels.Citilink;
using PriceTracker.Models.DTOModels.ForParsing;
using Microsoft.Playwright;
using PriceTracker.Models.Services.ScrapingServices.HttpClients.Browser;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.ScrapingServices.ShopSpecificModels.Citilink;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ScrapingServices.ShopSpecific.Citilink;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink;

namespace PriceTrackerTest.Services.ScrapingServices.ShopSpecificModels.Citilink
{
    public class CitilinkMerchScraperParserTests
    {
        CitilinkMerchParser parser;
        public CitilinkMerchScraperParserTests()
        {
            var playwrightTask = Playwright.CreateAsync();
            var browser = playwrightTask.Result.Chromium.LaunchAsync().Result;
            var browserAdapter = new BrowserAdapter(browser, (3,6));
            var scraper = new CitilinkMerchScraper(browserAdapter);
            parser = new(scraper);
        }



        [Fact]
        void ParsePortion_WorksCorrectly()
        {
            //https://www.citilink.ru/catalog/sendvichnicy/?ref=mainmenu_plate
            
            List<CitilinkMerchParsingDto> merchesParsed = parser.
                ParsePortionFromUrl("https://www.citilink.ru/catalog/sendvichnicy/?ref=mainmenu_plate").Result;
            
            Assert.Equal(36, merchesParsed.Count());
            Assert.Equal<CitilinkMerchParsingDto>(new(2590, 431311.ToString(), "Вафельница KitFort KT-1620,  серебристый"), merchesParsed[0]);
        }
    }
}
