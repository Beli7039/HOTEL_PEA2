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

        // GET: /Cliente
        public async Task<IActionResult> Index(string? buscar)
        {
            var query = _context.Cliente.AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(c =>
                    c.Nombres.Contains(buscar) ||
                    c.Apellidos.Contains(buscar) ||
                    c.Dni.Contains(buscar));
            }

            var lista = await query.OrderBy(c => c.IdCliente).ToListAsync();
            ViewBag.Buscar = buscar;
            return View(lista);
        }

        // GET: /Cliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(c => c.IdCliente == id);

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
                cliente.FechaRegistro = DateTime.Now;
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
                    if (!_context.Cliente.Any(c => c.IdCliente == id))
                        return NotFound();
                    throw;
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
                .FirstOrDefaultAsync(c => c.IdCliente == id);

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
                _context.Cliente.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}