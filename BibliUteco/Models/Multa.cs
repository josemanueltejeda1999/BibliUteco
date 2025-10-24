using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliUteco.Models
{
    public class Multa
    {
        [Key]
        public int MultaId { get; set; }

        [Required]
        public int PrestamoId { get; set; }

        [Required]
        public DateTime FechaGenerada { get; set; } = DateTime.Now;

        [Required]
        public int DiasRetraso { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente/Pagada

        [StringLength(20)]
        public string? MetodoPago { get; set; } // Efectivo/Tarjeta/Transferencia

        public DateTime? FechaPago { get; set; }

        // Relaciones
        [ForeignKey("PrestamoId")]
        public virtual Prestamo? Prestamo { get; set; }
    }
}