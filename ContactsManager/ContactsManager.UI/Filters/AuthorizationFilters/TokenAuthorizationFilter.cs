using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDs.Filters.AuthorizationFilters
{
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(!context.HttpContext.Request.Cookies.ContainsKey("Auth-key")
                || context.HttpContext.Request.Cookies["Auth-key"] != "A100")
            {
                context.Result = new StatusCodeResult(401);
                return;
            }
        }
    }
}
