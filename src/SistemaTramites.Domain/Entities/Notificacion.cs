using System.ComponentModel.DataAnnotations;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Notificación enviada automáticamente al ciudadano (RF-09). Relación N..1 con Tramite (envía).
/// </summary>
public class Notificacion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un trámite.")]
    [Display(Name = "Trámite")]
    public int TramiteId { get; set; }
    public Tramite? Tramite { get; set; }

    [Required(ErrorMessage = "Indique el medio de envío.")]
    // Valores esperados: Correo, WhatsApp, SMS
    [StringLength(50)]
    public string Medio { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    // Valores esperados: Pendiente, Enviado, Fallido
    public string Estado { get; set; } = "Pendiente";
}
