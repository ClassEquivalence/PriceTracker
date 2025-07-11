using HtmlAgilityPack;
using Microsoft.Playwright;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine
{
    public interface ICitilinkScraper
    {
        public Task<HtmlNode> UrlToNode(string url);

        public HtmlNode HtmlToNode(string html);

        public Task<HtmlNode> ScrapProductPortionFromUrl(string url);
    }
}
