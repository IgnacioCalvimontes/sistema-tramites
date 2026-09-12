using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Application.Services;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Data;
using SistemaTramites.Infrastructure.Identity;
using SistemaTramites.Web.Models;

namespace SistemaTramites.Web.Controllers;

/// <summary>
/// Portal del ciudadano solicitante (actor "Ciudadano" del diagrama de casos de uso):
/// registra sus propias solicitudes y consulta el estado de sus trámites (RF-01, RF-04).
/// Cada acción filtra por el CiudadanoId de la cuenta logueada, de modo que un ciudadano
/// nunca puede ver el expediente de otro.
/// </summary>
[Authorize(Roles = RolesApp.Ciudadano)]
public class MiPortalController : Controller
{
    private readonly AppDbContext _context;
    private readonly ITramiteService _tramiteService;
    private readonly UserManager<ApplicationUser> _userManager;

    public MiPortalController(
        AppDbContext context,
        ITramiteService tramiteService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _tramiteService = tramiteService;
        _userManager = userManager;
    }

    /// <summary>Ficha de ciudadano asociada a la cuenta actual (null si la cuenta no está vinculada).</summary>
    private async Task<int?> ObtenerCiudadanoIdAsync()
    {
        var cuenta = await _userManager.GetUserAsync(User);
        return cuenta?.CiudadanoId;
    }

    // GET: MiPortal — "Mis trámites"
    public async Task<IActionResult> Index()
    {
        var ciudadanoId = await ObtenerCiudadanoIdAsync();
        if (ciudadanoId == null)
            return View("CuentaSinVincular");

        var ficha = await _context.Ciudadanos.FindAsync(ciudadanoId.Value);
        if (ficha == null)
            return View("CuentaSinVincular");

        var tramites = await _context.Tramites
            .Include(t => t.TipoTramite)
            .Include(t => t.Pagos)
            .Where(t => t.CiudadanoId == ciudadanoId.Value)
            .OrderByDescending(t => t.FechaSolicitud)
            .ThenByDescending(t => t.Id)
            .Select(t => new TramiteCiudadanoViewModel
            {
                Id = t.Id,
                CodigoSeguimiento = t.CodigoSeguimiento,
                TipoTramite = t.TipoTramite!.Nombre,
                FechaSolicitud = t.FechaSolicitud,
                Estado = t.Estado,
                MontoArancel = t.TipoTramite.CostoArancel,
                Pagado = t.Pagos.Any(),
            })
            .ToListAsync();

        return View(new MiPortalViewModel
        {
            NombreCiudadano = ficha.Nombre,
            CI = ficha.CI,
            Tramites = tramites,
        });
    }

    // GET: MiPortal/NuevaSolicitud
    [HttpGet]
    public async Task<IActionResult> NuevaSolicitud()
    {
        var ciudadanoId = await ObtenerCiudadanoIdAsync();
        if (ciudadanoId == null)
            return View("CuentaSinVincular");

        return View(new NuevaSolicitudViewModel
        {
            TiposDisponibles = await _context.TiposTramite.OrderBy(t => t.Nombre).ToListAsync(),
        });
    }

    // POST: MiPortal/NuevaSolicitud
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NuevaSolicitud(NuevaSolicitudViewModel model)
    {
        var ciudadanoId = await ObtenerCiudadanoIdAsync();
        if (ciudadanoId == null)
            return View("CuentaSinVincular");

        if (!ModelState.IsValid)
        {
            model.TiposDisponibles = await _context.TiposTramite.OrderBy(t => t.Nombre).ToListAsync();
            return View(model);
        }

        try
        {
            // El ciudadanoId NO viene del formulario: se toma de la sesión, para que
            // nadie pueda registrar un trámite a nombre de otra persona.
            // Sin oficial asignado: lo asigna la notaría al recepcionar el expediente.
            var resultado = await _tramiteService.RegistrarSolicitudAsync(
                ciudadanoId.Value, model.TipoTramiteId, oficialId: null);

            TempData["Mensaje"] =
                $"Solicitud registrada. Su código de seguimiento es {resultado.CodigoSeguimiento} " +
                $"y el arancel a pagar es Bs {resultado.MontoAPagar:N2}. Acérquese a Caja para cancelarlo.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.TiposDisponibles = await _context.TiposTramite.OrderBy(t => t.Nombre).ToListAsync();
            return View(model);
        }
    }

    // GET: MiPortal/Detalle/5 — solo si el trámite le pertenece
    public async Task<IActionResult> Detalle(int? id)
    {
        if (id == null) return NotFound();

        var ciudadanoId = await ObtenerCiudadanoIdAsync();
        if (ciudadanoId == null)
            return View("CuentaSinVincular");

        var tramite = await _context.Tramites
            .Include(t => t.TipoTramite)
            .Include(t => t.Pagos)
            .Include(t => t.Citas)
            .Include(t => t.Notificaciones)
            .FirstOrDefaultAsync(t => t.Id == id && t.CiudadanoId == ciudadanoId.Value);

        // Si el trámite existe pero es de otro ciudadano, cae igualmente en NotFound:
        // no se revela la existencia de expedientes ajenos.
        if (tramite == null) return NotFound();

        return View(tramite);
    }
}
