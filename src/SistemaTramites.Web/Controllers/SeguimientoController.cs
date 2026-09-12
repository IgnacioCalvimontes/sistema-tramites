using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Infrastructure.Data;
using SistemaTramites.Web.Models;

namespace SistemaTramites.Web.Controllers;

/// <summary>
/// Consulta pública del estado de un trámite por código de seguimiento (RF-04).
/// No requiere iniciar sesión: es la ventanilla virtual para el ciudadano que
/// solo tiene el comprobante con su código.
/// </summary>
[AllowAnonymous]
public class SeguimientoController : Controller
{
    private readonly AppDbContext _context;

    public SeguimientoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? codigo)
    {
        // Permite tanto entrar en blanco como llegar con ?codigo=TR-2026-00001
        if (string.IsNullOrWhiteSpace(codigo))
            return View(new SeguimientoViewModel());

        return View(await BuscarAsync(codigo));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SeguimientoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        return View(await BuscarAsync(model.Codigo!));
    }

    private async Task<SeguimientoViewModel> BuscarAsync(string codigo)
    {
        var buscado = codigo.Trim();

        var tramite = await _context.Tramites
            .Include(t => t.TipoTramite)
            .Include(t => t.Ciudadano)
            .Include(t => t.Pagos)
            .FirstOrDefaultAsync(t => t.CodigoSeguimiento == buscado);

        var vm = new SeguimientoViewModel { Codigo = buscado, Buscado = true };

        if (tramite == null)
            return vm;

        vm.Resultado = new TramiteCiudadanoViewModel
        {
            Id = tramite.Id,
            CodigoSeguimiento = tramite.CodigoSeguimiento,
            TipoTramite = tramite.TipoTramite?.Nombre ?? "-",
            FechaSolicitud = tramite.FechaSolicitud,
            Estado = tramite.Estado,
            MontoArancel = tramite.TipoTramite?.CostoArancel ?? 0m,
            Pagado = tramite.Pagos.Any(),
        };

        // Se muestra solo el primer nombre para no exponer datos personales completos
        // en una pantalla sin autenticación.
        var nombre = tramite.Ciudadano?.Nombre ?? string.Empty;
        vm.NombreCiudadano = nombre.Split(' ').FirstOrDefault();

        return vm;
    }
}
