using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliUteco.Models
{
    public class Prestamo
    {
        [Key]
        public int PrestamoId { get; set; }

        [Required]
        public int LibroId { get; set; }

        [Required]
        public int EstudianteId { get; set; }

        [Required]
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        [Required]
        public DateTime FechaDevolucionEsperada { get; set; }

        public DateTime? FechaDevolucionReal { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Prestado"; // Prestado, Devuelto, Atrasado

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MultaPorRetraso { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("LibroId")]
        public virtual Libro? Libro { get; set; }

        [ForeignKey("EstudianteId")]
        public virtual Estudiante? Estudiante { get; set; }

        // Propiedades computadas
        public bool EstaAtrasado =>
            FechaDevolucionReal == null &&
            DateTime.Now > FechaDevolucionEsperada;

        public int DiasRetraso =>
            FechaDevolucionReal == null && DateTime.Now > FechaDevolucionEsperada
                ? (DateTime.Now - FechaDevolucionEsperada).Days
                : 0;
    }
}