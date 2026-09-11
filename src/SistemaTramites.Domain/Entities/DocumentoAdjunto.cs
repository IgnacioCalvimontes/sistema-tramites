using System.ComponentModel.DataAnnotations;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Documento digitalizado adjuntado a un trámite (RF-08). Relación N..1 con Tramite (incluye).
/// </summary>
public class DocumentoAdjunto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un trámite.")]
    [Display(Name = "Trámite")]
    public int TramiteId { get; set; }
    public Tramite? Tramite { get; set; }

    [Required(ErrorMessage = "Indique el tipo de documento.")]
    [Display(Name = "Tipo de documento")]
    [StringLength(120)]
    public string TipoDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indique la ruta del archivo.")]
    [Display(Name = "Ruta del archivo")]
    [StringLength(300)]
    public string ArchivoRuta { get; set; } = string.Empty;
}
