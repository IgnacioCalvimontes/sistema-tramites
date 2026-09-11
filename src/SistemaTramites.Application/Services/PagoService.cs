using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Application.Services;

public class PagoService : IPagoService
{
    private readonly AppDbContext _context;

    public PagoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrarPagoResultado> RegistrarPagoAsync(int tramiteId, int cajeroId, decimal monto)
    {
        var tramite = await _context.Tramites
            .Include(t => t.Ciudadano)
            .FirstOrDefaultAsync(t => t.Id == tramiteId)
            ?? throw new InvalidOperationException("El trámite indicado no existe.");

        var cajero = await _context.Usuarios.FindAsync(cajeroId)
            ?? throw new InvalidOperationException("El cajero indicado no existe.");

        // 1) Registrar pago (RF-05).
        var pago = new Pago
        {
            TramiteId = tramiteId,
            CajeroId = cajeroId,
            Monto = monto,
            FechaPago = DateTime.Now,
        };
        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        // 2) Emitir recibo: número correlativo simple basado en el Id del pago.
        pago.NumeroRecibo = $"REC-{DateTime.Now:yyyy}-{pago.Id:D6}";

        // 3) Actualizar estado del trámite.
        tramite.Estado = "Pagado";

        // 4) Disparar notificación al ciudadano (RF-09).
        var notificacion = new Notificacion
        {
            TramiteId = tramiteId,
            Medio = "Correo",
            Estado = "Enviado",
        };
        _context.Notificaciones.Add(notificacion);

        await _context.SaveChangesAsync();

        return new RegistrarPagoResultado
        {
            PagoId = pago.Id,
            NumeroRecibo = pago.NumeroRecibo,
            Monto = pago.Monto,
            CodigoSeguimientoTramite = tramite.CodigoSeguimiento,
            NombreCiudadano = tramite.Ciudadano?.Nombre ?? string.Empty,
            NotificacionEnviada = true,
        };
    }
}
