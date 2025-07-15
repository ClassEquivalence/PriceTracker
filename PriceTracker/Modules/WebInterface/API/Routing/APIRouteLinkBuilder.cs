using PriceTracker.Modules.WebInterface.API.Controllers.ForAdmin;

namespace PriceTracker.Modules.WebInterface.API.Routing
{
    public class APIRouteLinkBuilder
    {
        public static ControllerRoutes ControllerRoutes = new();


        public APIRouteLinkBuilder(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }
        private readonly LinkGenerator _linkGenerator;

        public string GetShopMerchesPath(int shopId)
        {
            var link = _linkGenerator.GetPathByAction
                (
                action: nameof(AdminMerchController.Get),
                controller: nameof(AdminMerchController),
                values: new { id = shopId }
                );
            return link == null ? throw new InvalidOperationException("Не удалось сгенерировать ссылку") : link;
        }
    }
}
