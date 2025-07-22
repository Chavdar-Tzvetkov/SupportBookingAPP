using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SupportBookingAPP.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/500")]
        public IActionResult Error500()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewData["Path"] = exceptionFeature?.Path;
            ViewData["Error"] = exceptionFeature?.Error?.Message;
            return View("Error500");
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return statusCode switch
            {
                404 => View("Error404"),
                403 => View("Error403"),
                _ => View("ErrorGeneric", model: statusCode)
            };
        }
    }
}
