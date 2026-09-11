using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Application.Services;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

/// <summary>
/// Casos de uso operativos de punta a punta, tal como los describen
/// los diagramas de secuencia del documento de diseño:
///   1) Registrar solicitud de trámite (RF-01, RF-03)
///   2) Registrar pago y emitir recibo (RF-05, RF-09)
/// </summary>
public class OperacionesController : Controller
{
    private readonly AppDbContext _context;
    private readonly ITramiteService _tramiteService;
    private readonly IPagoService _pagoService;

    public OperacionesController(AppDbContext context, ITramiteService tramiteService, IPagoService pagoService)
    {
        _context = context;
        _tramiteService = tramiteService;
        _pagoService = pagoService;
    }

    // ---------- 1) Registrar solicitud de trámite ----------

    [HttpGet]
    public IActionResult RegistrarSolicitud()
    {
        CargarListasSolicitud();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarSolicitud(int ciudadanoId, int tipoTramiteId, int? oficialId)
    {
        try
        {
            var resultado = await _tramiteService.RegistrarSolicitudAsync(ciudadanoId, tipoTramiteId, oficialId);
            return View("ResultadoSolicitud", resultado);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CargarListasSolicitud();
            return View();
        }
    }

    private void CargarListasSolicitud()
    {
        ViewData["CiudadanoId"] = new SelectList(_context.Ciudadanos, "Id", "Nombre");
        ViewData["TipoTramiteId"] = new SelectList(_context.TiposTramite, "Id", "Nombre");
        ViewData["OficialId"] = new SelectList(_context.Usuarios.Where(u => u.Rol == "Oficial" || u.Rol == "Ventanilla"), "Id", "Nombre");
    }

    // ---------- 2) Registrar pago y emitir recibo ----------

    [HttpGet]
    public async Task<IActionResult> RegistrarPago()
    {
        await CargarListasPagoAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPago(int tramiteId, int cajeroId, decimal monto)
    {
        try
        {
            var resultado = await _pagoService.RegistrarPagoAsync(tramiteId, cajeroId, monto);
            return View("Recibo", resultado);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarListasPagoAsync();
            return View();
        }
    }

    private async Task CargarListasPagoAsync()
    {
        // Solo trámites que aún no están pagados, con su monto de arancel visible.
        var pendientes = await _context.Tramites
            .Include(t => t.TipoTramite)
            .Include(t => t.Ciudadano)
            .Where(t => t.Estado == "Registrado")
            .Select(t => new
            {
                t.Id,
                Etiqueta = t.CodigoSeguimiento + " — " + t.Ciudadano!.Nombre + " (" + t.TipoTramite!.Nombre + ", Bs " + t.TipoTramite.CostoArancel + ")"
            })
            .ToListAsync();

        ViewData["TramiteId"] = new SelectList(pendientes, "Id", "Etiqueta");
        ViewData["CajeroId"] = new SelectList(_context.Usuarios.Where(u => u.Rol == "Caja"), "Id", "Nombre");
    }
}
