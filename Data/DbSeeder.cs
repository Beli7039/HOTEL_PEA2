using HOTEL_PEA2.Models;
using Microsoft.EntityFrameworkCore;

namespace HOTEL_PEA2.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // ============================================================
            // 1) TIPOS DE HABITACIÓN
            // ============================================================
            if (!await context.Tipo_Habitacion.AnyAsync())
            {
                context.Tipo_Habitacion.AddRange(
                    new Tipo_Habitacion { NombreTipo = "Simple", Descripcion = "Habitacion individual con una cama", PrecioBase = 50.00m, Capacidad = 2 },
                    new Tipo_Habitacion { NombreTipo = "Doble", Descripcion = "Habitacion con dos camas", PrecioBase = 90.00m, Capacidad = 3 },
                    new Tipo_Habitacion { NombreTipo = "Suite", Descripcion = "Habitacion de lujo con sala", PrecioBase = 180.00m, Capacidad = 4 }
                );
                await context.SaveChangesAsync();
            }

            // ============================================================
            // 2) HABITACIONES
            // ============================================================
            if (!await context.Habitacion.AnyAsync())
            {
                var simple = await context.Tipo_Habitacion.FirstAsync(t => t.NombreTipo == "Simple");
                var doble = await context.Tipo_Habitacion.FirstAsync(t => t.NombreTipo == "Doble");
                var suite = await context.Tipo_Habitacion.FirstAsync(t => t.NombreTipo == "Suite");

                context.Habitacion.AddRange(
                    new Habitacion { Numero = "101", Piso = "1", Capacidad = 2, Precio = 50.00m, IdTipoHabitacion = simple.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "102", Piso = "1", Capacidad = 2, Precio = 50.00m, IdTipoHabitacion = simple.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "103", Piso = "1", Capacidad = 2, Precio = 50.00m, IdTipoHabitacion = simple.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "104", Piso = "1", Capacidad = 2, Precio = 50.00m, IdTipoHabitacion = simple.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "105", Piso = "1", Capacidad = 2, Precio = 50.00m, IdTipoHabitacion = simple.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "201", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "202", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "203", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "204", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "205", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "206", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "207", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "208", Piso = "2", Capacidad = 3, Precio = 90.00m, IdTipoHabitacion = doble.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "301", Piso = "3", Capacidad = 4, Precio = 180.00m, IdTipoHabitacion = suite.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "302", Piso = "3", Capacidad = 4, Precio = 180.00m, IdTipoHabitacion = suite.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "303", Piso = "3", Capacidad = 4, Precio = 180.00m, IdTipoHabitacion = suite.IdTipoHabitacion, Estado = "Disponible" },
                    new Habitacion { Numero = "304", Piso = "3", Capacidad = 4, Precio = 180.00m, IdTipoHabitacion = suite.IdTipoHabitacion, Estado = "Disponible" }
                );
                await context.SaveChangesAsync();
            }

            // ============================================================
            // 3) CLIENTES
            // ============================================================
            if (!await context.Cliente.AnyAsync())
            {
                context.Cliente.AddRange(
                    new Cliente { Dni = "41258963", Nombres = "Carlos Alberto", Apellidos = "Quispe Mamani", Telefono = "987654321", Email = "carlos.quispe@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "42369874", Nombres = "Jose Luis", Apellidos = "Huaman Torres", Telefono = "986543210", Email = "jose.huaman@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "43147852", Nombres = "Miguel Angel", Apellidos = "Rojas Paredes", Telefono = "985432109", Email = "miguel.rojas@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "44785236", Nombres = "Rosa Maria", Apellidos = "Flores Chavez", Telefono = "984321098", Email = "rosa.flores@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "45896321", Nombres = "Luis Enrique", Apellidos = "Vargas Soto", Telefono = "983210987", Email = "luis.vargas@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "46523478", Nombres = "Maria Elena", Apellidos = "Sanchez Rios", Telefono = "982109876", Email = "maria.sanchez@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "47145632", Nombres = "Pedro Pablo", Apellidos = "Castillo Luna", Telefono = "981098765", Email = "pedro.castillo@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "48236985", Nombres = "Ana Lucia", Apellidos = "Ramos Bravo", Telefono = "980987654", Email = "ana.ramos@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "49321475", Nombres = "Jorge Antonio", Apellidos = "Salazar Cordova", Telefono = "979876543", Email = "jorge.salazar@email.com", Estado = "ACTIVO" },
                    new Cliente { Dni = "50478521", Nombres = "Carmen Rosa", Apellidos = "Diaz Herrera", Telefono = "978765432", Email = "carmen.diaz@email.com", Estado = "ACTIVO" }
                );
                await context.SaveChangesAsync();
            }

            // ============================================================
            // 4) RECEPCIONISTAS
            // ============================================================
            if (!await context.Recepcionista.AnyAsync())
            {
                context.Recepcionista.AddRange(
                    new Recepcionista { Nombres = "Admin", Apellidos = "Sistema", Usuario = "admin", Contrasena = "admin123", Telefono = "962000001", Email = "admin@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Sandro", Apellidos = "Gomez", Usuario = "sgomez", Contrasena = "sgomez2026", Telefono = "962000002", Email = "sgomez@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Lucia", Apellidos = "Fernandez", Usuario = "lfernandez", Contrasena = "lfernandez26", Telefono = "962000003", Email = "lfernandez@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Roberto", Apellidos = "Mendoza", Usuario = "rmendoza", Contrasena = "rmendoza2026", Telefono = "962000004", Email = "rmendoza@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Patricia", Apellidos = "Silva", Usuario = "psilva", Contrasena = "psilva2026", Telefono = "962000005", Email = "psilva@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Andres", Apellidos = "Chavez", Usuario = "achavez", Contrasena = "achavez2026", Telefono = "962000006", Email = "achavez@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Veronica", Apellidos = "Ramos", Usuario = "vramos", Contrasena = "vramos2026", Telefono = "962000007", Email = "vramos@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Fernando", Apellidos = "Torres", Usuario = "ftorres", Contrasena = "ftorres2026", Telefono = "962000008", Email = "ftorres@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Gabriela", Apellidos = "Paredes", Usuario = "gparedes", Contrasena = "gparedes2026", Telefono = "962000009", Email = "gparedes@hotel.com", Estado = "ACTIVO" },
                    new Recepcionista { Nombres = "Ricardo", Apellidos = "Vega", Usuario = "rvega", Contrasena = "rvega2026", Telefono = "962000010", Email = "rvega@hotel.com", Estado = "ACTIVO" }
                );
                await context.SaveChangesAsync();
            }

            // ============================================================
            // 5) USUARIOS
            // ============================================================
            if (!await context.Usuario.AnyAsync())
            {
                context.Usuario.AddRange(
                    new Usuario { Nombre = "Administrador del Sistema", UserName = "admin", Clave = "admin123", Rol = "Administrador", Estado = true },
                    new Usuario { Nombre = "Sandro Gomez", UserName = "sgomez", Clave = "sgomez2026", Rol = "Administrador", Estado = true },
                    new Usuario { Nombre = "Lucia Fernandez", UserName = "lfernandez", Clave = "lfernandez26", Rol = "Recepcionista", Estado = true },
                    new Usuario { Nombre = "Roberto Mendoza", UserName = "rmendoza", Clave = "rmendoza2026", Rol = "Recepcionista", Estado = true },
                    new Usuario { Nombre = "Patricia Silva", UserName = "psilva", Clave = "psilva2026", Rol = "Recepcionista", Estado = true },
                    new Usuario { Nombre = "Andres Chavez", UserName = "achavez", Clave = "achavez2026", Rol = "Recepcionista", Estado = true },
                    new Usuario { Nombre = "Veronica Ramos", UserName = "vramos", Clave = "vramos2026", Rol = "Recepcionista", Estado = true },
                    new Usuario { Nombre = "Fernando Torres", UserName = "ftorres", Clave = "ftorres2026", Rol = "Recepcionista", Estado = true },
                    new Usuario { Nombre = "Gabriela Paredes", UserName = "gparedes", Clave = "gparedes2026", Rol = "Recepcionista", Estado = true },
                    new Usuario { Nombre = "Ricardo Vega", UserName = "rvega", Clave = "rvega2026", Rol = "Recepcionista", Estado = false }
                );
                await context.SaveChangesAsync();
            }

            // ============================================================
            // 6) RESERVAS
            // ============================================================
            if (!await context.Reserva.AnyAsync())
            {
                var clientes = await context.Cliente.OrderBy(c => c.IdCliente).ToListAsync();
                var habitaciones = await context.Habitacion.OrderBy(h => h.IdHabitacion).ToListAsync();
                var recepcionistas = await context.Recepcionista.OrderBy(r => r.IdRecepcionista).ToListAsync();

                if (clientes.Count >= 10 && habitaciones.Count >= 16 && recepcionistas.Count >= 1)
                {
                    var hoy = DateTime.Today;

                    context.Reserva.AddRange(
                        new Reserva { FechaEntrada = hoy.AddDays(1), FechaSalida = hoy.AddDays(3), CantidadPersonas = 1, CostoTotal = 100.00m, Estado = "Confirmada", TipoHabitacion = "Simple", IdCliente = clientes[0].IdCliente, IdHabitacion = habitaciones[0].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Cliente frecuente" },
                        new Reserva { FechaEntrada = hoy.AddDays(2), FechaSalida = hoy.AddDays(3), CantidadPersonas = 2, CostoTotal = 50.00m, Estado = "Pendiente", TipoHabitacion = "Simple", IdCliente = clientes[1].IdCliente, IdHabitacion = habitaciones[1].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Llegada tarde" },
                        new Reserva { FechaEntrada = hoy.AddDays(5), FechaSalida = hoy.AddDays(8), CantidadPersonas = 2, CostoTotal = 270.00m, Estado = "Confirmada", TipoHabitacion = "Doble", IdCliente = clientes[2].IdCliente, IdHabitacion = habitaciones[5].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Solicita cuna" },
                        new Reserva { FechaEntrada = hoy.AddDays(7), FechaSalida = hoy.AddDays(9), CantidadPersonas = 3, CostoTotal = 180.00m, Estado = "Pendiente", TipoHabitacion = "Doble", IdCliente = clientes[3].IdCliente, IdHabitacion = habitaciones[6].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Requiere estacionamiento" },
                        new Reserva { FechaEntrada = hoy.AddDays(10), FechaSalida = hoy.AddDays(14), CantidadPersonas = 2, CostoTotal = 720.00m, Estado = "Confirmada", TipoHabitacion = "Suite", IdCliente = clientes[4].IdCliente, IdHabitacion = habitaciones[13].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Luna de miel" },
                        new Reserva { FechaEntrada = hoy.AddDays(3), FechaSalida = hoy.AddDays(5), CantidadPersonas = 4, CostoTotal = 360.00m, Estado = "Cancelada", TipoHabitacion = "Suite", IdCliente = clientes[5].IdCliente, IdHabitacion = habitaciones[14].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Cancelada" },
                        new Reserva { FechaEntrada = hoy.AddDays(4), FechaSalida = hoy.AddDays(5), CantidadPersonas = 1, CostoTotal = 50.00m, Estado = "Confirmada", TipoHabitacion = "Simple", IdCliente = clientes[6].IdCliente, IdHabitacion = habitaciones[2].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Viaje de negocios" },
                        new Reserva { FechaEntrada = hoy.AddDays(15), FechaSalida = hoy.AddDays(18), CantidadPersonas = 2, CostoTotal = 270.00m, Estado = "Pendiente", TipoHabitacion = "Doble", IdCliente = clientes[7].IdCliente, IdHabitacion = habitaciones[7].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Confirmar pago" },
                        new Reserva { FechaEntrada = hoy.AddDays(20), FechaSalida = hoy.AddDays(22), CantidadPersonas = 2, CostoTotal = 100.00m, Estado = "Confirmada", TipoHabitacion = "Simple", IdCliente = clientes[8].IdCliente, IdHabitacion = habitaciones[3].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Late check-out" },
                        new Reserva { FechaEntrada = hoy.AddDays(25), FechaSalida = hoy.AddDays(30), CantidadPersonas = 3, CostoTotal = 900.00m, Estado = "Confirmada", TipoHabitacion = "Suite", IdCliente = clientes[9].IdCliente, IdHabitacion = habitaciones[15].IdHabitacion, IdRecepcionista = recepcionistas[0].IdRecepcionista, Observaciones = "Estadia larga" }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}