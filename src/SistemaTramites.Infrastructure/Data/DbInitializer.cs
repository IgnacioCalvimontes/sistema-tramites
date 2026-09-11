using SistemaTramites.Domain.Entities;

namespace SistemaTramites.Infrastructure.Data;

/// <summary>
/// Crea la base de datos SQLite (si no existe) y la llena con datos de ejemplo
/// para poder probar el sistema de inmediato.
/// </summary>
public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.TiposTramite.Any())
            return; // ya hay datos

        var tipos = new[]
        {
            new TipoTramite { Nombre = "Poder Notarial", CostoArancel = 150.00m, Plantilla = "plantillas/poder_notarial.docx" },
            new TipoTramite { Nombre = "Testimonio de Escritura", CostoArancel = 220.00m, Plantilla = "plantillas/testimonio.docx" },
            new TipoTramite { Nombre = "Certificación de Firma", CostoArancel = 60.00m, Plantilla = "plantillas/certificacion_firma.docx" },
        };
        context.TiposTramite.AddRange(tipos);

        var usuarios = new[]
        {
            new Usuario { Nombre = "Mariana Rojas", Rol = "Ventanilla", UsuarioLogin = "mrojas" },
            new Usuario { Nombre = "Carlos Fernández", Rol = "Oficial", UsuarioLogin = "cfernandez" },
            new Usuario { Nombre = "Lucía Paredes", Rol = "Caja", UsuarioLogin = "lparedes" },
            new Usuario { Nombre = "Dr. Álvaro Montes", Rol = "Notario", UsuarioLogin = "amontes" },
            new Usuario { Nombre = "Jorge Salazar", Rol = "Archivo", UsuarioLogin = "jsalazar" },
        };
        context.Usuarios.AddRange(usuarios);

        var ciudadanos = new[]
        {
            new Ciudadano { CI = "5487621", Nombre = "Beatriz Choque", Contacto = "70011223" },
            new Ciudadano { CI = "3321980", Nombre = "Ramiro Quispe", Contacto = "ramiro.quispe@mail.com" },
        };
        context.Ciudadanos.AddRange(ciudadanos);

        context.SaveChanges();
    }
}
