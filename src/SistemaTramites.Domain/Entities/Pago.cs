using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Pago del arancel de un trámite, con emisión de recibo (RF-05).
/// Relación N..1 con Tramite (genera) y N..1 con Usuario/cajero (registra).
/// </summary>
public class Pago
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un trámite.")]
    [Display(Name = "Trámite")]
    public int TramiteId { get; set; }
    public Tramite? Tramite { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el cajero que registra el pago.")]
    [Display(Name = "Cajero")]
    public int CajeroId { get; set; }
    public Usuario? Cajero { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(0.01, 999999)]
    public decimal Monto { get; set; }

    [Display(Name = "Número de recibo")]
    [StringLength(30)]
    public string NumeroRecibo { get; set; } = string.Empty;

    [Display(Name = "Fecha de pago")]
    public DateTime FechaPago { get; set; } = DateTime.Now;
}
