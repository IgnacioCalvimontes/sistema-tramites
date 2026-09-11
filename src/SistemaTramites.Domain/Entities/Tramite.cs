using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Entidad central del sistema. Relaciona ciudadano, tipo de trámite y oficial,
/// y se ramifica hacia citas, pagos, documentos, protocolo y notificaciones.
/// </summary>
public class Tramite
{
    public int Id { get; set; }

    [Display(Name = "Código de seguimiento")]
    [StringLength(30)]
    public string CodigoSeguimiento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un ciudadano.")]
    [Display(Name = "Ciudadano solicitante")]
    public int CiudadanoId { get; set; }
    public Ciudadano? Ciudadano { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de trámite.")]
    [Display(Name = "Tipo de trámite")]
    public int TipoTramiteId { get; set; }
    public TipoTramite? TipoTramite { get; set; }

    [Display(Name = "Oficial responsable")]
    public int? OficialId { get; set; }
    public Usuario? Oficial { get; set; }

    [Required]
    [Display(Name = "Fecha de solicitud")]
    [DataType(DataType.Date)]
    public DateTime FechaSolicitud { get; set; } = DateTime.Today;

    [Required]
    [StringLength(30)]
    // Valores esperados: Registrado, Pagado, En proceso, Protocolizado, Finalizado
    public string Estado { get; set; } = "Registrado";

    [NotMapped]
    [Display(Name = "Monto a pagar (Bs)")]
    public decimal MontoArancel => TipoTramite?.CostoArancel ?? 0m;

    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<DocumentoAdjunto> Documentos { get; set; } = new List<DocumentoAdjunto>();
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    public Protocolo? Protocolo { get; set; }
}
