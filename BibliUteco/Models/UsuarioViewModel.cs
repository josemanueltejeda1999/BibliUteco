using System.ComponentModel.DataAnnotations;

namespace BibliUteco.Models
{
    public class UsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nombre de usuario")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        public string? Rol { get; set; }

        [Display(Name = "Email confirmado")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Acceso bloqueado")]
        public bool LockoutEnabled { get; set; }

        public DateTime? LockoutEnd { get; set; }
    }

    public class AsignarRolViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public string RolSeleccionado { get; set; } = string.Empty;

        public List<string>? RolesDisponibles { get; set; }
        public string? RolActual { get; set; }
    }

    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;
    }
}