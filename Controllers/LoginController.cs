using HOTEL_PEA2.Data;
using Microsoft.AspNetCore.Mvc;

namespace HOTEL_PEA2.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string usuario, string clave)
        {
            var usuarioEncontrado = _context.Usuario
                .FirstOrDefault(x =>
                x.UserName == usuario &&
                x.Clave == clave &&
                x.Estado == true);

            if (usuarioEncontrado != null)
            {
                HttpContext.Session.SetString(
                    "Usuario",
                    usuarioEncontrado.UserName);

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";

            return View();
        }
    }
}
