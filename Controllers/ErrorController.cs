using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Trans9.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /*[Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    _logger.LogWarning($"404 Error Occured.\r\nPath: {statusCodeResult.OriginalPath} thew an exception \r\n" +
                                    $"QueryString: {statusCodeResult.OriginalQueryString}");
                    ViewBag.errorMessage = "Sory, The requested resources could not found...";
                    ViewBag.path = statusCodeResult.OriginalPath;
                    ViewBag.queryString = statusCodeResult.OriginalQueryString;
                    break;
                default:
                    break;
            }
            return View("NotFound");
        }*/

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionResult = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            _logger.LogError($"Path: {exceptionResult.Path} thew an exception \r\n" +
                $"ErrorMessage: {exceptionResult.Error.Message} \r\n" +
                $"StackTrace :{exceptionResult.Error.StackTrace}");

            ViewBag.path = exceptionResult.Path;
            ViewBag.errorMessage = exceptionResult.Error.Message;
            ViewBag.stackTrace = exceptionResult.Error.StackTrace;

            return View("Error");
        }
    }
}
