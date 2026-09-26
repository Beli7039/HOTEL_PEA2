using HOTEL_PEA2.Data;
using HOTEL_PEA2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HOTEL_PEA2.Controllers
{
    public class RecepcionistaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecepcionistaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Recepcionista.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var r = await _context.Recepcionista.FindAsync(id);
            if (r == null) return NotFound();
            return View(r);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recepcionista r)
        {
            if (ModelState.IsValid)
            {
                _context.Add(r);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(r);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var r = await _context.Recepcionista.FindAsync(id);
            if (r == null) return NotFound();
            return View(r);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Recepcionista r)
        {
            if (id != r.IdRecepcionista) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(r);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(r);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var r = await _context.Recepcionista.FindAsync(id);
            if (r == null) return NotFound();
            return View(r);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _context.Recepcionista.FindAsync(id);
            if (r != null)
            {
                _context.Recepcionista.Remove(r);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}