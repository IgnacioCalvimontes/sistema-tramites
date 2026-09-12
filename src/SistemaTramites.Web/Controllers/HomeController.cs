using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

/// <summary>
/// Tablero de inicio. Cada rol aterriza en la pantalla que le corresponde:
/// el ciudadano en su portal, el personal en el tablero interno de la notaría.
/// </summary>
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // El ciudadano no ve las estadísticas internas de la notaría: va a "Mis trámites".
        if (User.IsInRole(RolesApp.Ciudadano))
            return RedirectToAction("Index", "MiPortal");

        ViewBag.TotalCiudadanos = await _context.Ciudadanos.CountAsync();
        ViewBag.TotalTramites = await _context.Tramites.CountAsync();
        ViewBag.TramitesPendientes = await _context.Tramites.CountAsync(t => t.Estado == "Registrado");
        ViewBag.TramitesPagados = await _context.Tramites.CountAsync(t => t.Estado == "Pagado");

        // Recaudación acumulada: dato de supervisión para el Notario.
        // SQLite no soporta SUM sobre decimal, así que se agrega en memoria.
        var montos = await _context.Pagos.Select(p => p.Monto).ToListAsync();
        ViewBag.TotalRecaudado = montos.Sum();
        ViewBag.TotalProtocolos = await _context.Protocolos.CountAsync();

        return View();
    }

    public IActionResult Error() => View();
}
