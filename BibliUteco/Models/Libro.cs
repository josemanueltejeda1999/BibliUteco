using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliUteco.Models
{
    public class Libro
    {
        [Key]
        public int LibroId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ISBN es obligatorio")]
        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        public int AutorId { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "La editorial es obligatoria")]
        [StringLength(100)]
        public string Editorial { get; set; } = string.Empty;

        // Nombre sin tilde mapeado por EF
        public int? AnoPublicacion { get; set; }

        // Alias con tilde para compatibilidad con las vistas que usan "AñoPublicacion"
        [NotMapped]
        public int? AñoPublicacion
        {
            get => AnoPublicacion;
            set => AnoPublicacion = value;
        }

        [Required]
        public int CantidadTotal { get; set; }

        [Required]
        public int CantidadDisponible { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [StringLength(500)]
        public string? ImagenUrl { get; set; }

        [StringLength(50)]
        public string? Idioma { get; set; }

        public int? NumeroPaginas { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("AutorId")]
        public virtual Autor? Autor { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        public virtual ICollection<Prestamo>? Prestamos { get; set; }

        // Propiedad computada
        [NotMapped]
        public bool EstaDisponible => CantidadDisponible > 0;
    }
}