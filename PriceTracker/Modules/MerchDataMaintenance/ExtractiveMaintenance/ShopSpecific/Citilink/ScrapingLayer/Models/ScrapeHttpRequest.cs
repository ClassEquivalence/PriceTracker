
using System.Diagnostics.CodeAnalysis;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models
{
    public class ScrapeHttpRequest : HttpRequestMessage
    {
        private ScrapeHttpRequest(HttpMethod method, [StringSyntax("Uri")] string requestUri) :
            base(method, requestUri)
        {
        }

        public static ScrapeHttpRequest From(CitilinkScrapeEpisodeDescriptor descriptor)
        {
            HttpMethod method = descriptor.Method;
            string uri = descriptor.Uri;
            ScrapeHttpRequest request = new ScrapeHttpRequest(method, uri);
            if (descriptor.Cookies.Count > 0)
                request.AddCookies(descriptor.Cookies);

            if (descriptor.RequestBody != null && descriptor.RequestBodyEncoding != null
                && descriptor.RequestBodyMediaType != null)
            {
                request.Content = new StringContent(descriptor.RequestBody,
                    descriptor.RequestBodyEncoding, descriptor.RequestBodyMediaType);
            }
            return request;
        }

        private void AddCookies(Dictionary<string, string> cookies)
        {
            if (cookies.Count <= 0)
                throw new ArgumentException("Словарь кук не должен быть пуст", paramName: nameof(cookies));
            string cookieString = string.Join("; ", cookies.Select(p => $"{p.Key}={p.Value}"));
            if (Headers.Contains("Cookie"))
            {
                Headers.TryGetValues("Cookie", out var values);
                var currentCookie = values.FirstOrDefault();

                // 2. Склеиваем старое и новое через точу с запятой
                var updatedCookie = $"{currentCookie}; {cookieString}";

                // 3. Удаляем старое и записываем обновленное
                Headers.Remove("Cookie");
                Headers.Add("Cookie", updatedCookie);
            }
            else
            {
                Headers.Add("Cookie", cookieString);
            }
        }
    }
}
