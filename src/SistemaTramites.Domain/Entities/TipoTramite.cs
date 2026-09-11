using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Tipo de trámite: define el costo de arancel (RF-03) y la plantilla a usar (RF-07).
/// Relación 1..N con Tramite (clasifica).
/// </summary>
public class TipoTramite
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del tipo de trámite es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Costo de arancel (Bs)")]
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 999999)]
    public decimal CostoArancel { get; set; }

    [Display(Name = "Plantilla del documento")]
    [StringLength(200)]
    public string? Plantilla { get; set; }

    public ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
