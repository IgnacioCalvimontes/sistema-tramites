using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalCiudadanos = await _context.Ciudadanos.CountAsync();
        ViewBag.TotalTramites = await _context.Tramites.CountAsync();
        ViewBag.TramitesPendientes = await _context.Tramites.CountAsync(t => t.Estado == "Registrado");
        ViewBag.TramitesPagados = await _context.Tramites.CountAsync(t => t.Estado == "Pagado");
        return View();
    }

    public IActionResult Error() => View();
}
