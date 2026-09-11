using System.ComponentModel.DataAnnotations;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Registro de protocolización/archivo de un trámite (RF-10). Relación 0..1 con Tramite (archiva).
/// </summary>
public class Protocolo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un trámite.")]
    [Display(Name = "Trámite")]
    public int TramiteId { get; set; }
    public Tramite? Tramite { get; set; }

    [Required]
    [Display(Name = "Año")]
    [Range(2000, 2100)]
    public int Anio { get; set; } = DateTime.Today.Year;

    [Required(ErrorMessage = "Indique el libro.")]
    [StringLength(50)]
    public string Libro { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indique el folio.")]
    [StringLength(50)]
    public string Folio { get; set; } = string.Empty;
}
