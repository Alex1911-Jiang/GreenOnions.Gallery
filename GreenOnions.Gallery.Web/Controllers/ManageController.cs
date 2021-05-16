using GreenOnions.Gallery.Api;
using GreenOnions.Gallery.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GreenOnions.Gallery.Web.Controllers
{
    [AuthFilter]
    public class ManageController : Controller
    {
        private readonly ILogger<ManageController> _logger;

        public ManageController(ILogger<ManageController> logger, IConfiguration configuration)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddPicture()
        {
            return View();
        }

        public IActionResult EditPicture(string defOrigin)
        {
            ViewBag.Origin = defOrigin;
            return View();
        }

        public IActionResult AddPictureRange()
        {
            return View();
        }
    }
}
