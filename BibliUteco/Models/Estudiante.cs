using System.ComponentModel.DataAnnotations;

namespace BibliUteco.Models
{
    public class Estudiante
    {
        [Key]
        public int EstudianteId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es obligatoria")]
        [StringLength(20)]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Teléfono inválido")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Carrera { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación con Préstamos
        public virtual ICollection<Prestamo>? Prestamos { get; set; }

        // Propiedad computada
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}