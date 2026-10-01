using Microsoft.AspNetCore.Mvc.Rendering;
using HOTEL_PEA2.Models;

namespace HOTEL_PEA2.Models.ViewModels
{
    /// <summary>
    /// ViewModel que agrupa todos los datos necesarios para la vista
    /// "Nueva Reserva":
    ///   - Los 3 modelos principales: Reserva, Cliente, Habitacion
    ///   - Las listas para los <select>
    ///   - Cálculos auxiliares (precio noche, noches, total)
    /// </summary>
    public class ReservaViewModel
    {
        // ============================================================
        // MODELOS PRINCIPALES
        // Se inicializan con "new()" para que nunca sean null.
        // Esto evita NullReferenceException si el modelo binder no
        // recibe datos (por ejemplo, cuando el usuario no envía algún
        // campo o cuando la vista se carga por primera vez).
        // ============================================================

        /// <summary>Datos de la reserva (fechas, personas, estado, etc.)</summary>
        public Reserva Reserva { get; set; } = new Reserva();

        /// <summary>Datos del cliente (existente o nuevo).</summary>
        public Cliente Cliente { get; set; } = new Cliente();

        /// <summary>Datos de la habitación (precio, piso, número).</summary>
        public Habitacion Habitacion { get; set; } = new Habitacion();

        // ============================================================
        // LISTAS PARA LOS <select>
        // También se inicializan como listas vacías para que el
        // controlador y la vista puedan iterarlas sin riesgo.
        // ============================================================

        /// <summary>Lista de clientes existentes (para el combo).</summary>
        public List<SelectListItem> ListaClientes { get; set; } = new();

        /// <summary>Lista de habitaciones disponibles (para el combo).</summary>
        public List<SelectListItem> ListaHabitaciones { get; set; } = new();

        /// <summary>Lista de tipos de habitación (para el combo).</summary>
        public List<SelectListItem> ListaTipoHabitacion { get; set; } = new();

        // ============================================================
        // CÁLCULOS AUXILIARES
        // El JavaScript los calcula en cliente, pero tenerlos aquí
        // permite validarlos en el servidor antes de guardar.
        // ============================================================

        /// <summary>Precio por noche de la habitación seleccionada.</summary>
        public decimal PrecioNoche { get; set; }

        /// <summary>Cantidad de noches entre FechaEntrada y FechaSalida.</summary>
        public int CantidadNoches { get; set; }

        /// <summary>Costo total calculado (PrecioNoche × CantidadNoches).</summary>
        public decimal CostoTotal { get; set; }
    }
}