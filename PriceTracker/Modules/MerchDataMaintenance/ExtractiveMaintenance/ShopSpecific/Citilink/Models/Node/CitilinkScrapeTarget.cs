using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using System.Net;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node
{
    // Функциональное назначение содержимого контента с точки зрения приложения
    public enum CitilinkScrapeTargetPurpose { Default, MerchesJson }
    public class CitilinkScrapeTarget
    {
        // Корректно ли processed? Мб лучше visited?
        public bool IsProcessed;
        public string Url;

        public CitilinkScrapeTargetPurpose Purpose;

        public CitilinkScrapeTarget(string url, CitilinkScrapeTargetPurpose purpose
            = CitilinkScrapeTargetPurpose.Default, bool processed = false)
        {
            Url = url;
            IsProcessed = processed;
            Purpose = purpose;
        }

        public virtual CitilinkScrapeEpisodeDescriptor CreateDescriptor()
        {
            if (Purpose == CitilinkScrapeTargetPurpose.Default)
            {
                CitilinkScrapeEpisodeDescriptor descriptor = new(Url, HttpMethod.Get,
                    [HttpStatusCode.OK]);
                return descriptor;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

    }
}
