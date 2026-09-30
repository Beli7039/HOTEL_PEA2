using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HOTEL_PEA2.Data;          // Para acceder a ApplicationDbContext
using Microsoft.EntityFrameworkCore; // Para usar CountAsync, SumAsync, etc.
using System.Globalization;     // Para formatear la fecha en español

namespace HOTEL_PEA2.Controllers
{
    public class HomeController : Controller
    {
        // Guardamos el contexto de la base de datos en un campo privado.
        // Gracias a la inyección de dependencias de ASP.NET Core,
        // el contexto llega automáticamente al constructor.
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // ACCIÓN PRINCIPAL DEL DASHBOARD
        // ============================================================
        public async Task<IActionResult> Dashboard()
        {
            // --- 1. FECHA ACTUAL EN ESPAÑOL ---
            // Creamos una cultura peruana para que el mes salga en español
            var cultura = new CultureInfo("es-PE");
            ViewBag.FechaActual = DateTime.Now.ToString(
                "dd 'de' MMMM 'de' yyyy", cultura);

            // --- 2. USUARIO LOGUEADO ---
            // Leemos la sesión que guardó LoginController.
            // Si por alguna razón está vacía, mostramos "Invitado".
            ViewBag.Usuario = HttpContext.Session.GetString("Usuario") ?? "Invitado";

            // --- 3. ESTADÍSTICAS DE HABITACIONES ---
            // Contamos cuántas habitaciones tienen cada estado.
            // Se hace una sola consulta por estado para que sea eficiente.
            ViewBag.TotalHabitaciones = await _context.Habitacion.CountAsync();
            ViewBag.HabitacionesDisponibles = await _context.Habitacion
                .CountAsync(h => h.Estado == "Disponible");
            ViewBag.HabitacionesOcupadas = await _context.Habitacion
                .CountAsync(h => h.Estado == "Ocupada");
            ViewBag.HabitacionesMantenimiento = await _context.Habitacion
                .CountAsync(h => h.Estado == "Mantenimiento");
            ViewBag.HabitacionesFueraServicio = await _context.Habitacion
                .CountAsync(h => h.Estado == "Fuera de servicio");

            // --- 4. PORCENTAJE DE OCUPACIÓN ---
            // Calculamos el % usando una regla de tres simple.
            // Si no hay habitaciones, evitamos dividir por cero.
            ViewBag.PorcentajeOcupacion = ViewBag.TotalHabitaciones > 0
                ? (ViewBag.HabitacionesOcupadas * 100) / ViewBag.TotalHabitaciones
                : 0;

            // --- 5. RESERVAS DE HOY ---
            // DateTime.Today es la fecha sin hora (00:00 hoy).
            // Comparamos la parte de fecha con .Date para ignorar la hora.
            var hoy = DateTime.Today;
            ViewBag.ReservasHoy = await _context.Reserva
                .CountAsync(r => r.FechaEntrada.Date == hoy);

            // --- 6. CHECK-IN DE HOY ---
            // Reservas cuya entrada es hoy y aún no han hecho check-in real.
            ViewBag.CheckInHoy = await _context.Reserva
                .CountAsync(r => r.FechaEntrada.Date == hoy
                              && (r.Estado == "Reservado" || r.Estado == "Confirmado"));

            // --- 7. CHECK-OUT DE HOY ---
            // Reservas que salen hoy y están "En Curso".
            ViewBag.CheckOutHoy = await _context.Reserva
                .CountAsync(r => r.FechaSalida.Date == hoy
                              && r.Estado == "En Curso");

            // --- 8. INGRESOS DEL DÍA ---
            // SUMAMOS el CostoTotal de las reservas de hoy.
            // SumAsync devuelve decimal? (nullable), por eso usamos ?? 0
            // para convertir null en 0 si no hay reservas.
            ViewBag.IngresosHoy = await _context.Reserva
                .Where(r => r.FechaEntrada.Date == hoy)
                .SumAsync(r => (decimal?)r.CostoTotal) ?? 0;

            // --- 9. PRÓXIMAS LLEGADAS (3 más cercanas) ---
            // Reservas cuya entrada es hoy o más adelante.
            // Include trae la info relacionada (Cliente y Habitacion)
            // para no hacer varias consultas después.
            ViewBag.ProximasLlegadas = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .Where(r => r.FechaEntrada.Date >= hoy)
                .OrderBy(r => r.FechaEntrada)
                .Take(3)
                .ToListAsync();

            // --- 10. OCUPACIÓN ÚLTIMOS 7 DÍAS (para el gráfico) ---
            // Para cada uno de los últimos 7 días, contamos
            // cuántas reservas estaban activas ese día.
            var labels = new List<string>();   // Etiquetas del eje X
            var valores = new List<int>();     // Valores del eje Y

            for (int i = 6; i >= 0; i--)
            {
                var dia = hoy.AddDays(-i);

                // Una reserva está activa en 'dia' si:
                //   FechaEntrada <= dia  Y  FechaSalida >= dia
                int activas = await _context.Reserva
                    .CountAsync(r => r.FechaEntrada.Date <= dia
                                  && r.FechaSalida.Date >= dia);

                labels.Add(dia.ToString("dd MMM", cultura));
                valores.Add(activas);
            }

            // ViewBag es un contenedor dinámico donde guardamos datos
            // para que la vista (.cshtml) los pueda leer.
            ViewBag.GraficoLabels = labels;
            ViewBag.GraficoValores = valores;

            return View();
        }

        // Acción que devuelve un PartialView (por si la usas en AJAX)
        public IActionResult DashboardContenido()
        {
            return PartialView("_DashboardHome");
        }
    }
}