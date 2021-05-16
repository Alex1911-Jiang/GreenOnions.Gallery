using Microsoft.AspNetCore.Mvc.Filters;

namespace GreenOnions.Gallery.Api.Utility
{
    public class AccessFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");  //允许跨域
            base.OnActionExecuting(context);
        }
    }
}
