using Microsoft.AspNetCore.Identity;

namespace SistemaTramites.Infrastructure.Identity;

/// <summary>
/// Cuenta de acceso a la aplicación web (login), separada del catálogo de negocio
/// "Usuario" (que representa al personal como oficial/cajero en los trámites y pagos).
/// El rol de acceso (Administrador, Notario, Ventanilla, Caja, Oficial, Archivo) se
/// gestiona con los roles de ASP.NET Core Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
}
