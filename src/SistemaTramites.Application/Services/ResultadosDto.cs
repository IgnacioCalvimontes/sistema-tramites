namespace SistemaTramites.Application.Services;

/// <summary>Resultado de registrar una solicitud de trámite (RF-01, RF-03).</summary>
public class RegistrarSolicitudResultado
{
    public int TramiteId { get; set; }
    public string CodigoSeguimiento { get; set; } = string.Empty;
    public decimal MontoAPagar { get; set; }
    public string NombreCiudadano { get; set; } = string.Empty;
    public string NombreTipoTramite { get; set; } = string.Empty;
}

/// <summary>Resultado de registrar un pago y emitir recibo (RF-05, RF-09).</summary>
public class RegistrarPagoResultado
{
    public int PagoId { get; set; }
    public string NumeroRecibo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string CodigoSeguimientoTramite { get; set; } = string.Empty;
    public string NombreCiudadano { get; set; } = string.Empty;
    public bool NotificacionEnviada { get; set; }
}
