using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SIGC_PROJECT.Helper
{
    public class FiltroAutenticacion : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "PrincipalPage", null);
            }
        }
    }
}
