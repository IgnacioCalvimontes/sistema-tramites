using System.ComponentModel.DataAnnotations;

namespace SistemaTramites.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su usuario.")]
    [Display(Name = "Usuario")]
    public string UsuarioLogin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Mantener sesión iniciada")]
    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}
