using System.Net;
using System.Text;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner
{
    public class CitilinkScrapeEpisodeDescriptor
    {
        public CitilinkScrapeEpisodeDescriptor(string uri, HttpMethod method,
            List<HttpStatusCode> allowedStatuses, Dictionary<string, string>? cookies = null,
            string? requestBody = null, Encoding? requestBodyEncoding = null,
            string? requestBodyMediaType = null)
        {
            Uri = uri;
            Method = method;
            AllowedStatuses = allowedStatuses;
            Cookies = cookies ?? [];

            RequestBody = requestBody;
            RequestBodyEncoding = requestBodyEncoding;
            RequestBodyMediaType = requestBodyMediaType;
        }
        // Допущение: скрапинг происходит только через http(s) протокол.
        public string Uri;
        public HttpMethod Method;
        public List<HttpStatusCode> AllowedStatuses;
        public Dictionary<string, string> Cookies { get; }


        // TODO: вынести в одну структуру чтоль...
        public string? RequestBody;
        public Encoding? RequestBodyEncoding;
        public string? RequestBodyMediaType;
        // TODO: одна структура - конец.
        // А можно разделить дескриптор на запрос-инфу и ответ-инфу

        public override string ToString()
        {
            string allowedStatusesString = "";
            AllowedStatuses.ForEach(s => allowedStatusesString += s.ToString());

            string cookies = "";
            foreach (var item in Cookies)
            {
                cookies += $"{item.Key}: {item.Value}; ";
            }

            return $"{nameof(CitilinkScrapeEpisodeDescriptor)}'s fields:\n" +
                $"Uri: {Uri}\n Method: {Method}\n" +
                $"AllowedStatuses: {allowedStatusesString}\n" +
                $"Cookies: {cookies}\n" +
                $"RequestBody: {RequestBody} \n" +
                $"RequestBodyEncoding: {RequestBodyEncoding} \n" +
                $"RequestBodyMediaType: {RequestBodyMediaType}";
        }
    }
}
