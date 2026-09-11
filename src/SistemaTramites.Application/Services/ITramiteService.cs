namespace SistemaTramites.Application.Services;

public interface ITramiteService
{
    /// <summary>
    /// Implementa el diagrama de secuencia "Registrar solicitud de trámite" (RF-01, RF-03):
    /// Ciudadano -> Ventanilla -> Sistema: registra el trámite, calcula el arancel
    /// según el tipo de trámite y devuelve el código de seguimiento y el monto a pagar.
    /// </summary>
    Task<RegistrarSolicitudResultado> RegistrarSolicitudAsync(int ciudadanoId, int tipoTramiteId, int? oficialId);
}
