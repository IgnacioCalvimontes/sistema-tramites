using System.ComponentModel.DataAnnotations;
using SistemaTramites.Domain.Entities;

namespace SistemaTramites.Web.Models;

/// <summary>Fila del listado "Mis trámites" del portal del ciudadano.</summary>
public class TramiteCiudadanoViewModel
{
    public int Id { get; set; }
    public string CodigoSeguimiento { get; set; } = string.Empty;
    public string TipoTramite { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal MontoArancel { get; set; }
    public bool Pagado { get; set; }
}

/// <summary>Datos del portal del ciudadano (RF-04: consultar estado).</summary>
public class MiPortalViewModel
{
    public string NombreCiudadano { get; set; } = string.Empty;
    public string CI { get; set; } = string.Empty;
    public List<TramiteCiudadanoViewModel> Tramites { get; set; } = new();

    public int TotalTramites => Tramites.Count;
    public int PendientesDePago => Tramites.Count(t => !t.Pagado);
    public decimal SaldoPendiente => Tramites.Where(t => !t.Pagado).Sum(t => t.MontoArancel);
}

/// <summary>Formulario con el que el ciudadano solicita un trámite a su propio nombre.</summary>
public class NuevaSolicitudViewModel
{
    [Required(ErrorMessage = "Debe elegir el tipo de trámite que necesita.")]
    [Display(Name = "Tipo de trámite")]
    public int TipoTramiteId { get; set; }

    public IEnumerable<TipoTramite> TiposDisponibles { get; set; } = new List<TipoTramite>();
}

/// <summary>Consulta pública de estado por código de seguimiento (RF-04), sin iniciar sesión.</summary>
public class SeguimientoViewModel
{
    [Required(ErrorMessage = "Ingrese el código de seguimiento que le entregó la notaría.")]
    [Display(Name = "Código de seguimiento")]
    [StringLength(30)]
    public string? Codigo { get; set; }

    /// <summary>True una vez que se ejecutó una búsqueda (para distinguir "sin buscar" de "sin resultados").</summary>
    public bool Buscado { get; set; }

    public TramiteCiudadanoViewModel? Resultado { get; set; }
    public string? NombreCiudadano { get; set; }
}
