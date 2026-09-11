using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Application.Services;

public class TramiteService : ITramiteService
{
    private readonly AppDbContext _context;

    public TramiteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrarSolicitudResultado> RegistrarSolicitudAsync(int ciudadanoId, int tipoTramiteId, int? oficialId)
    {
        var ciudadano = await _context.Ciudadanos.FindAsync(ciudadanoId)
            ?? throw new InvalidOperationException("El ciudadano indicado no existe.");

        var tipoTramite = await _context.TiposTramite.FindAsync(tipoTramiteId)
            ?? throw new InvalidOperationException("El tipo de trámite indicado no existe.");

        // Calcular arancel (RF-03): tomado directamente del tipo de trámite.
        var montoArancel = tipoTramite.CostoArancel;

        var tramite = new Tramite
        {
            CiudadanoId = ciudadanoId,
            TipoTramiteId = tipoTramiteId,
            OficialId = oficialId,
            FechaSolicitud = DateTime.Today,
            Estado = "Registrado",
        };

        _context.Tramites.Add(tramite);
        await _context.SaveChangesAsync();

        // Generar código de seguimiento único a partir del Id ya asignado por la BD.
        tramite.CodigoSeguimiento = $"TR-{DateTime.Today:yyyy}-{tramite.Id:D5}";
        await _context.SaveChangesAsync();

        return new RegistrarSolicitudResultado
        {
            TramiteId = tramite.Id,
            CodigoSeguimiento = tramite.CodigoSeguimiento,
            MontoAPagar = montoArancel,
            NombreCiudadano = ciudadano.Nombre,
            NombreTipoTramite = tipoTramite.Nombre,
        };
    }
}
