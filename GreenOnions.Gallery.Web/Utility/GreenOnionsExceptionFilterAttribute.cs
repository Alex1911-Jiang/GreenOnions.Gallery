using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace GreenOnions.Gallery.Web.Utility
{
    public class GreenOnionsExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<GreenOnionsExceptionFilterAttribute> _logger;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        public GreenOnionsExceptionFilterAttribute(ILogger<GreenOnionsExceptionFilterAttribute> logger, IModelMetadataProvider modelMetadataProvider)
        {
            _modelMetadataProvider = modelMetadataProvider;
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                this._logger.LogError($"{context.HttpContext.Request.RouteValues["controller"]}发生了一个异常:{context.Exception.Message}");
                
                string header = context.HttpContext.Request.Headers["X-Requested-With"];
                if ("XMLHttpRequest".Equals(header))  //是否为ajax请求
                {
                    context.Result = new JsonResult(new
                    {
                        Result = false,
                        Msg = context.Exception.Message
                    });
                }
                else
                {
                    var result = new ViewResult { ViewName = "~/Views/Shared/Error.cshtml" };

                    //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

                    result.ViewData = new (_modelMetadataProvider, context.ModelState);
                    result.ViewData.Add("Exception", context.Exception);
                    result.ViewData.Add("Message", context.Exception.Message);
                    context.Result = result;
                }
                context.ExceptionHandled = true;
            }
        }
    }
}
