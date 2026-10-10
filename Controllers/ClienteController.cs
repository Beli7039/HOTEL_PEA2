using HOTEL_PEA2.Data;
using HOTEL_PEA2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HOTEL_PEA2.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: /Cliente
        // Lista de clientes ordenados por apellido y nombre.
        // AsNoTracking mejora rendimiento porque es solo lectura.
        // ============================================================
        public async Task<IActionResult> Index(string criterio)
        {
            var query = _context.Cliente
                .AsNoTracking()
                .Where(c => c.Estado == "ACTIVO") 
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(criterio))
            {
                query = query.Where(c =>
                    c.Dni.Contains(criterio) ||
                    c.Nombres.Contains(criterio) ||
                    c.Apellidos.Contains(criterio));
            }

            var clientes = await query
                .OrderBy(c => c.Apellidos)
                .ThenBy(c => c.Nombres)
                .ToListAsync();

            ViewBag.Criterio = criterio;
            return View(clientes);
        }

        // GET: /Cliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Cliente
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdCliente == id);

            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // GET: /Cliente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Validar DNI único
                var existeDni = await _context.Cliente
                    .AnyAsync(c => c.Dni == cliente.Dni);

                if (existeDni)
                {
                    ModelState.AddModelError("Dni", "Ya existe un cliente con ese DNI.");
                    return View(cliente);
                }

                cliente.FechaRegistro = DateTime.Now;
                cliente.Estado = "ACTIVO";

                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: /Cliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: /Cliente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.IdCliente)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: /Cliente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Cliente
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdCliente == id);

            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: /Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente != null)
            {
                cliente.Estado = "Inactivo"; 
                _context.Update(cliente);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.IdCliente == id);
        }
    }
}