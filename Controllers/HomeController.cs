using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
namespace HOTEL_PEA2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult DashboardContenido()
        {
            return PartialView("_DashboardHome");
        }

        public IActionResult Reservas()
        {
            return View();
        }
    }
}