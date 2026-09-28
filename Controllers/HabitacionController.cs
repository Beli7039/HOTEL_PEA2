using HOTEL_PEA2.Data;
using HOTEL_PEA2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HOTEL_PEA2.Controllers
{
    public class HabitacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HabitacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Habitacion
        public async Task<IActionResult> Index()
        {
            var habitaciones = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .ToListAsync();
            return View(habitaciones);
        }

        // GET: /Habitacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var habitacion = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .FirstOrDefaultAsync(m => m.IdHabitacion == id);
            if (habitacion == null) return NotFound();
            return View(habitacion);
        }

        // GET: /Habitacion/Create
        public IActionResult Create()
        {
            ViewBag.IdTipoHabitacion = new SelectList(
                _context.Tipo_Habitacion, "IdTipoHabitacion", "NombreTipo");
            return View();
        }

        // POST: /Habitacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Habitacion habitacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(habitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdTipoHabitacion = new SelectList(
                _context.Tipo_Habitacion, "IdTipoHabitacion", "NombreTipo", habitacion.IdTipoHabitacion);
            return View(habitacion);
        }

        // GET: /Habitacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var habitacion = await _context.Habitacion.FindAsync(id);
            if (habitacion == null) return NotFound();
            ViewBag.IdTipoHabitacion = new SelectList(
                _context.Tipo_Habitacion, "IdTipoHabitacion", "NombreTipo", habitacion.IdTipoHabitacion);
            return View(habitacion);
        }

        // POST: /Habitacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Habitacion habitacion)
        {
            if (id != habitacion.IdHabitacion) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacionExists(habitacion.IdHabitacion)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdTipoHabitacion = new SelectList(
                _context.Tipo_Habitacion, "IdTipoHabitacion", "NombreTipo", habitacion.IdTipoHabitacion);
            return View(habitacion);
        }

        // GET: /Habitacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var habitacion = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .FirstOrDefaultAsync(m => m.IdHabitacion == id);
            if (habitacion == null) return NotFound();
            return View(habitacion);
        }

        // POST: /Habitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacion = await _context.Habitacion.FindAsync(id);
            if (habitacion != null)
            {
                _context.Habitacion.Remove(habitacion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacionExists(int id)
        {
            return _context.Habitacion.Any(e => e.IdHabitacion == id);
        }
    }
}