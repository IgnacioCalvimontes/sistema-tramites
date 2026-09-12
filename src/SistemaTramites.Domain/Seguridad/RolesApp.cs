namespace SistemaTramites.Domain.Seguridad;

/// <summary>
/// Catálogo único de roles de acceso y de las combinaciones que se usan en
/// [Authorize(Roles = ...)]. Centralizarlos evita "strings mágicos" repetidos
/// por los controladores y garantiza que la matriz de permisos coincida con el
/// diagrama de casos de uso del documento de diseño.
/// </summary>
public static class RolesApp
{
    // ---------- Roles individuales (uno por actor del diagrama de casos de uso) ----------

    /// <summary>Administra cuentas y parámetros del sistema.</summary>
    public const string Administrador = "Administrador";

    /// <summary>Da fe pública, supervisa reportes y administra roles (RF-06).</summary>
    public const string Notario = "Notario";

    /// <summary>Atiende al público y registra solicitudes (RF-01).</summary>
    public const string Ventanilla = "Ventanilla";

    /// <summary>Cobra aranceles y emite recibos (RF-05, RF-09).</summary>
    public const string Caja = "Caja";

    /// <summary>Oficial que tramita el expediente asignado.</summary>
    public const string Oficial = "Oficial";

    /// <summary>Indexa y resguarda el protocolo notarial.</summary>
    public const string Archivo = "Archivo";

    /// <summary>Ciudadano solicitante: registra y consulta sus propios trámites.</summary>
    public const string Ciudadano = "Ciudadano";

    /// <summary>Todos los roles que se crean en el seed de Identity.</summary>
    public static readonly string[] Todos =
    {
        Administrador, Notario, Ventanilla, Caja, Oficial, Archivo, Ciudadano
    };

    // ---------- Combinaciones (grupos funcionales) ----------

    /// <summary>Personal interno de la notaría: todos menos el ciudadano.</summary>
    public const string Personal = $"{Administrador},{Notario},{Ventanilla},{Caja},{Oficial},{Archivo}";

    /// <summary>Mesa operativa: recepción y tramitación del expediente.</summary>
    public const string MesaOperativa = $"{Administrador},{Ventanilla},{Oficial}";

    /// <summary>Mesa operativa más archivo (gestión documental del expediente).</summary>
    public const string MesaYArchivo = $"{Administrador},{Ventanilla},{Oficial},{Archivo}";

    /// <summary>Quienes pueden mover dinero: caja.</summary>
    public const string Recaudacion = $"{Administrador},{Caja}";

    /// <summary>Caja más supervisión: el notario consulta pagos pero no los registra.</summary>
    public const string RecaudacionYSupervision = $"{Administrador},{Caja},{Notario}";

    /// <summary>Archivo más supervisión: protocolización e indexado.</summary>
    public const string ArchivoYSupervision = $"{Administrador},{Archivo},{Notario}";

    /// <summary>Supervisión: administración de roles, aranceles y reportes.</summary>
    public const string Supervision = $"{Administrador},{Notario}";
}
