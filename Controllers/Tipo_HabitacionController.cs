using HOTEL_PEA2.Data;
using HOTEL_PEA2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HOTEL_PEA2.Controllers
{
    public class Tipo_HabitacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Tipo_HabitacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Tipo_Habitacion.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var t = await _context.Tipo_Habitacion.FindAsync(id);
            if (t == null) return NotFound();
            return View(t);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tipo_Habitacion t)
        {
            if (ModelState.IsValid)
            {
                _context.Add(t);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(t);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var t = await _context.Tipo_Habitacion.FindAsync(id);
            if (t == null) return NotFound();
            return View(t);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tipo_Habitacion t)
        {
            if (id != t.IdTipoHabitacion) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(t);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(t);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var t = await _context.Tipo_Habitacion.FindAsync(id);
            if (t == null) return NotFound();
            return View(t);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t = await _context.Tipo_Habitacion.FindAsync(id);
            if (t != null)
            {
                _context.Tipo_Habitacion.Remove(t);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}