using System.ComponentModel.DataAnnotations;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Cita agendada para un trámite (RF-02). Relación N..1 con Tramite (agenda).
/// </summary>
public class Cita
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un trámite.")]
    [Display(Name = "Trámite")]
    public int TramiteId { get; set; }
    public Tramite? Tramite { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Hora")]
    public TimeSpan Hora { get; set; }

    [Required(ErrorMessage = "Indique el tipo de cita.")]
    [Display(Name = "Tipo de cita")]
    [StringLength(80)]
    public string TipoCita { get; set; } = string.Empty;
}
