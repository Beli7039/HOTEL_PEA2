using Microsoft.AspNetCore.Mvc;

namespace ProyectoHotel.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}