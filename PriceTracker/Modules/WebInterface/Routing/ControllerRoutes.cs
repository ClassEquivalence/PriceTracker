namespace PriceTracker.Modules.WebInterface.Routing
{
    public class ControllerRoutes
    {
        private const string AdminControllerRoutePrefix =
            "api/admin";
        public const string AdminMerchControllerRoute =
            $"{AdminControllerRoutePrefix}/Merch";
        public const string AdminShopControllerRoute =
            $"{AdminControllerRoutePrefix}/Shop";

        private const string UserControllerRoutePrefix =
            "api";
        public const string UserMerchControllerRoute =
            $"{UserControllerRoutePrefix}/Merch";
    }
}
