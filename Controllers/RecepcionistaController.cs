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

        // ============================================================
        // GET: /Recepcionista
        // Lista de recepcionistas ordenados por apellido.
        // ============================================================
        public async Task<IActionResult> Index(string criterio)
        {
            var query = _context.Recepcionista.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(criterio))
            {
                query = query.Where(r =>
                    r.Nombres.Contains(criterio) ||
                    r.Apellidos.Contains(criterio) ||
                    r.Usuario.Contains(criterio));
            }

            var recepcionistas = await query
                .OrderBy(r => r.Apellidos)
                .ThenBy(r => r.Nombres)
                .ToListAsync();

            ViewBag.Criterio = criterio;
            return View(recepcionistas);
        }

        // GET: /Recepcionista/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var recepcionista = await _context.Recepcionista
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdRecepcionista == id);

            if (recepcionista == null) return NotFound();
            return View(recepcionista);
        }

        // GET: /Recepcionista/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Recepcionista/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recepcionista recepcionista)
        {
            if (ModelState.IsValid)
            {
                // Validar que el usuario no exista
                var existeUsuario = await _context.Recepcionista
                    .AnyAsync(r => r.Usuario == recepcionista.Usuario);

                if (existeUsuario)
                {
                    ModelState.AddModelError("Usuario", "Ya existe un recepcionista con ese usuario.");
                    return View(recepcionista);
                }

                recepcionista.Estado = "ACTIVO";

                _context.Add(recepcionista);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recepcionista);
        }

        // GET: /Recepcionista/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var recepcionista = await _context.Recepcionista.FindAsync(id);
            if (recepcionista == null) return NotFound();
            return View(recepcionista);
        }

        // POST: /Recepcionista/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Recepcionista recepcionista)
        {
            if (id != recepcionista.IdRecepcionista) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recepcionista);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecepcionistaExists(recepcionista.IdRecepcionista)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recepcionista);
        }

        // GET: /Recepcionista/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var recepcionista = await _context.Recepcionista
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdRecepcionista == id);

            if (recepcionista == null) return NotFound();
            return View(recepcionista);
        }

        // POST: /Recepcionista/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recepcionista = await _context.Recepcionista.FindAsync(id);
            if (recepcionista != null)
            {
                _context.Recepcionista.Remove(recepcionista);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RecepcionistaExists(int id)
        {
            return _context.Recepcionista.Any(e => e.IdRecepcionista == id);
        }
    }
}