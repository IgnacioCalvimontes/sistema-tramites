using System.ComponentModel.DataAnnotations;

namespace SistemaTramites.Domain.Entities;

/// <summary>
/// Personal interno del sistema: ventanilla, oficial, caja, notario o archivo (RF-06).
/// Un Usuario puede atender trámites (como oficial) o registrar pagos (como cajero).
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [StringLength(50)]
    // Valores esperados: Ventanilla, Caja, Oficial, Notario, Archivo
    public string Rol { get; set; } = string.Empty;

    [Required(ErrorMessage = "El usuario de acceso es obligatorio.")]
    [Display(Name = "Usuario de acceso")]
    [StringLength(100)]
    public string UsuarioLogin { get; set; } = string.Empty;

    public ICollection<Tramite> TramitesAtendidos { get; set; } = new List<Tramite>();
    public ICollection<Pago> PagosRegistrados { get; set; } = new List<Pago>();
}
