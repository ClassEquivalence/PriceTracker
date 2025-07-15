using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PriceTracker.Modules.WebInterface.API.Filters
{
    public class AdminAPIAuthorization : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
