using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Identity;

namespace SistemaTramites.Infrastructure.Data;

/// <summary>
/// Crea los roles del sistema y una cuenta de acceso por cada perfil descrito
/// en el diagrama de casos de uso, incluido el ciudadano solicitante.
/// </summary>
public static class IdentityDataInitializer
{
    public const string PasswordDemo = "Notaria2026";

    /// <summary>Roles de acceso; la lista vive en <see cref="RolesApp"/>.</summary>
    public static readonly string[] Roles = RolesApp.Todos;

    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        foreach (var rol in Roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        // Personal interno de la notaría: sin ficha de ciudadano asociada.
        var cuentas = new (string login, string nombre, string rol)[]
        {
            ("admin", "Administrador del sistema", RolesApp.Administrador),
            ("amontes", "Dr. Álvaro Montes", RolesApp.Notario),
            ("mrojas", "Mariana Rojas", RolesApp.Ventanilla),
            ("lparedes", "Lucía Paredes", RolesApp.Caja),
            ("cfernandez", "Carlos Fernández", RolesApp.Oficial),
            ("jsalazar", "Jorge Salazar", RolesApp.Archivo),
        };

        foreach (var (login, nombre, rol) in cuentas)
            await CrearCuentaAsync(userManager, login, nombre, rol, ciudadanoId: null);

        // Cuentas de ciudadano: se enlazan por CI con la ficha ya cargada por DbInitializer,
        // de modo que el portal "Mis trámites" tenga datos que mostrar en la demostración.
        var ciudadanos = new (string login, string ci)[]
        {
            ("bchoque", "5487621"),
            ("rquispe", "3321980"),
        };

        foreach (var (login, ci) in ciudadanos)
        {
            var ficha = await context.Ciudadanos.FirstOrDefaultAsync(c => c.CI == ci);
            if (ficha == null)
                continue;

            await CrearCuentaAsync(userManager, login, ficha.Nombre, RolesApp.Ciudadano, ficha.Id);
        }
    }

    private static async Task CrearCuentaAsync(
        UserManager<ApplicationUser> userManager,
        string login,
        string nombre,
        string rol,
        int? ciudadanoId)
    {
        if (await userManager.FindByNameAsync(login) != null)
            return;

        var user = new ApplicationUser
        {
            UserName = login,
            Email = $"{login}@notaria19.local",
            EmailConfirmed = true,
            NombreCompleto = nombre,
            CiudadanoId = ciudadanoId,
        };

        var resultado = await userManager.CreateAsync(user, PasswordDemo);
        if (resultado.Succeeded)
            await userManager.AddToRoleAsync(user, rol);
    }
}
