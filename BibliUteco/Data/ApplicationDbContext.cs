using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BibliUteco.Models;

namespace BibliUteco.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar índices únicos
            modelBuilder.Entity<Libro>()
                .HasIndex(l => l.ISBN)
                .IsUnique();

            modelBuilder.Entity<Estudiante>()
                .HasIndex(e => e.Matricula)
                .IsUnique();

            modelBuilder.Entity<Estudiante>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Configurar relaciones
            modelBuilder.Entity<Libro>()
                .HasOne(l => l.Autor)
                .WithMany(a => a.Libros)
                .HasForeignKey(l => l.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Libro>()
                .HasOne(l => l.Categoria)
                .WithMany(c => c.Libros)
                .HasForeignKey(l => l.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Libro)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(p => p.LibroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Estudiante)
                .WithMany(e => e.Prestamos)
                .HasForeignKey(p => p.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Datos semilla (seed data)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Categorías iniciales
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    CategoriaId = 1,
                    Nombre = "Ficción",
                    Descripcion = "Libros de ficción y narrativa",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Categoria
                {
                    CategoriaId = 2,
                    Nombre = "Ciencia",
                    Descripcion = "Libros científicos y académicos",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Categoria
                {
                    CategoriaId = 3,
                    Nombre = "Historia",
                    Descripcion = "Libros históricos y biografías",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Categoria
                {
                    CategoriaId = 4,
                    Nombre = "Tecnología",
                    Descripcion = "Libros de tecnología y programación",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Categoria
                {
                    CategoriaId = 5,
                    Nombre = "Literatura",
                    Descripcion = "Literatura clásica y contemporánea",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Categoria
                {
                    CategoriaId = 6,
                    Nombre = "Derecho",
                    Descripcion = "Libros de leyes y derecho",
                    FechaCreacion = new DateTime(2024, 1, 1)
                }
            );

            // Autores iniciales
            modelBuilder.Entity<Autor>().HasData(
                new Autor
                {
                    AutorId = 1,
                    Nombre = "Gabriel",
                    Apellido = "García Márquez",
                    Nacionalidad = "Colombiana",
                    FechaNacimiento = new DateTime(1927, 3, 6),
                    Biografia = "Escritor colombiano, Premio Nobel de Literatura 1982",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Autor
                {
                    AutorId = 2,
                    Nombre = "Isabel",
                    Apellido = "Allende",
                    Nacionalidad = "Chilena",
                    FechaNacimiento = new DateTime(1942, 8, 2),
                    Biografia = "Escritora chilena, autora de La casa de los espíritus",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Autor
                {
                    AutorId = 3,
                    Nombre = "Jorge",
                    Apellido = "Luis Borges",
                    Nacionalidad = "Argentina",
                    FechaNacimiento = new DateTime(1899, 8, 24),
                    Biografia = "Escritor argentino, figura clave de la literatura del siglo XX",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Autor
                {
                    AutorId = 4,
                    Nombre = "Mario",
                    Apellido = "Vargas Llosa",
                    Nacionalidad = "Peruana",
                    FechaNacimiento = new DateTime(1936, 3, 28),
                    Biografia = "Escritor peruano, Premio Nobel de Literatura 2010",
                    FechaCreacion = new DateTime(2024, 1, 1)
                }
            );
        }
    }
}