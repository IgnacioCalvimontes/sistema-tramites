namespace SistemaTramites.Application.Services;

public interface IPagoService
{
    /// <summary>
    /// Implementa el diagrama de secuencia "Registrar pago y emitir recibo" (RF-05, RF-09):
    /// Ciudadano -> Caja -> Sistema: registra el pago, actualiza el estado del trámite,
    /// emite el recibo y dispara la notificación al ciudadano.
    /// </summary>
    Task<RegistrarPagoResultado> RegistrarPagoAsync(int tramiteId, int cajeroId, decimal monto);
}
