using System.Globalization;
using HOTEL_PEA2.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// CONFIGURACIÓN DE CULTURA
// Se crea una cultura personalizada basada en "es-PE" pero con:
//   - Punto como separador decimal (50.00 en vez de 50,00)
//   - Coma como separador de miles (1,000 en vez de 1.000)
//   - Símbolo de moneda "S/"
//   - Fechas en formato dd/MM/yyyy
// Esto se aplica a TODA la aplicación (vistas, controllers, etc.)
// ============================================================
var culturaPeru = (CultureInfo)CultureInfo.GetCultureInfo("es-PE").Clone();
culturaPeru.NumberFormat.NumberDecimalSeparator = ".";
culturaPeru.NumberFormat.NumberGroupSeparator = ",";
culturaPeru.NumberFormat.CurrencyDecimalSeparator = ".";
culturaPeru.NumberFormat.CurrencyGroupSeparator = ",";
culturaPeru.NumberFormat.CurrencySymbol = "S/";
culturaPeru.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
culturaPeru.DateTimeFormat.LongDatePattern = "dddd, dd 'de' MMMM 'de' yyyy";

CultureInfo.DefaultThreadCurrentCulture = culturaPeru;
CultureInfo.DefaultThreadCurrentUICulture = culturaPeru;

// ============================================================
// SERVICIOS
// ============================================================

// MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// DbContext apuntando a la cadena de conexión "cn"
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("cn")));

// Sesiones (para login)
builder.Services.AddSession();

// ============================================================
// PIPELINE
// ============================================================
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// ============================================================
// SEED: cargar datos iniciales al arrancar la aplicación
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.Run();