using Microsoft.AspNetCore.Identity;
using SistemaTramites.Domain.Entities;

namespace SistemaTramites.Infrastructure.Identity;

/// <summary>
/// Cuenta de acceso a la aplicación web (login), separada del catálogo de negocio
/// "Usuario" (que representa al personal como oficial/cajero en los trámites y pagos).
/// El rol de acceso (Administrador, Notario, Ventanilla, Caja, Oficial, Archivo,
/// Ciudadano) se gestiona con los roles de ASP.NET Core Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Solo para cuentas del rol "Ciudadano": enlaza el login con su ficha en el
    /// catálogo de ciudadanos. Es lo que permite que el portal muestre únicamente
    /// SUS trámites y que al solicitar uno nuevo quede a su nombre.
    /// Las cuentas del personal interno lo dejan en null.
    /// </summary>
    public int? CiudadanoId { get; set; }
    public Ciudadano? Ciudadano { get; set; }
}
