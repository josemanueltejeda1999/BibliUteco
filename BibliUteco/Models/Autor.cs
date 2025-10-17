using System.ComponentModel.DataAnnotations;

namespace BibliUteco.Models
{
    public class Autor
    {
        [Key]
        public int AutorId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Nacionalidad { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(500)]
        public string? Biografia { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación con Libros
        public virtual ICollection<Libro>? Libros { get; set; }

        // Propiedad computada para nombre completo
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
