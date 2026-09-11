using Microsoft.AspNetCore.Identity;
using SistemaTramites.Infrastructure.Identity;

namespace SistemaTramites.Infrastructure.Data;

/// <summary>
/// Crea los roles del sistema y algunas cuentas de acceso de ejemplo,
/// una por cada perfil descrito en el diagrama de casos de uso.
/// </summary>
public static class IdentityDataInitializer
{
    public const string PasswordDemo = "Notaria2026";

    public static readonly string[] Roles =
    {
        "Administrador", "Notario", "Ventanilla", "Caja", "Oficial", "Archivo"
    };

    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        foreach (var rol in Roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        var cuentas = new (string login, string nombre, string rol)[]
        {
            ("admin", "Administrador del sistema", "Administrador"),
            ("amontes", "Dr. Álvaro Montes", "Notario"),
            ("mrojas", "Mariana Rojas", "Ventanilla"),
            ("lparedes", "Lucía Paredes", "Caja"),
            ("cfernandez", "Carlos Fernández", "Oficial"),
            ("jsalazar", "Jorge Salazar", "Archivo"),
        };

        foreach (var (login, nombre, rol) in cuentas)
        {
            if (await userManager.FindByNameAsync(login) != null)
                continue;

            var user = new ApplicationUser
            {
                UserName = login,
                Email = $"{login}@notaria19.local",
                EmailConfirmed = true,
                NombreCompleto = nombre,
            };

            var resultado = await userManager.CreateAsync(user, PasswordDemo);
            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(user, rol);
            }
        }
    }
}
