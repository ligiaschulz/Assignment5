using Microsoft.AspNetCore.Mvc;

namespace Assignment5.Controllers
{
    public class BrowseMusicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }
    }
}
