using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportBookingAPP.Attributes;

namespace SupportBookingAPP.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [AdminArea]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
