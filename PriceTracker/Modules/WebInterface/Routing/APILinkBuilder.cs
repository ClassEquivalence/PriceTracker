using Microsoft.AspNetCore.Routing;
using PriceTracker.Modules.WebInterface.Controllers.APIControllers;

namespace PriceTracker.Modules.WebInterface.Routing
{
    public class APILinkBuilder
    {
        public APILinkBuilder(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }
        private readonly LinkGenerator _linkGenerator;

        public string GetShopMerchesPath(int shopId)
        {
            var link = _linkGenerator.GetPathByAction
                (
                action: nameof(MerchController.Get),
                controller: nameof(MerchController),
                values: new { id = shopId }
                );
            return link == null ? throw new InvalidOperationException("Не удалось сгенерировать ссылку") : link;
        }
    }
}
