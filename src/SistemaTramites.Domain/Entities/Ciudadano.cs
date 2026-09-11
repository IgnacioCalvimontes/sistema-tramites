using System.ComponentModel.DataAnnotations;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Ciudadano que solicita uno o varios trámites (relación 1..N con Tramite).
/// </summary>
public class Ciudadano
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El CI es obligatorio.")]
    [Display(Name = "Carnet de Identidad")]
    [StringLength(20)]
    public string CI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Contacto (teléfono / correo)")]
    [StringLength(150)]
    public string? Contacto { get; set; }

    public ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
