using BibliUteco.Data;
using BibliUteco.Models;
using BibliUteco.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BibliUteco.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EstadisticasGenerales> ObtenerEstadisticasGeneralesAsync()
        {
            try
            {
                var estadisticas = new EstadisticasGenerales
                {
                    TotalLibros = await _context.Libros.CountAsync(l => l.Activo),
                    LibrosDisponibles = await _context.Libros.CountAsync(l => l.Activo && l.CantidadDisponible > 0),
                    LibrosPrestados = await _context.Libros
                        .Where(l => l.Activo)
                        .SumAsync(l => l.CantidadTotal - l.CantidadDisponible),

                    TotalEstudiantes = await _context.Estudiantes.CountAsync(e => e.Activo),
                    EstudiantesActivos = await _context.Prestamos
                        .Where(p => p.Estado == "Prestado" || p.Estado == "Atrasado")
                        .Select(p => p.EstudianteId)
                        .Distinct()
                        .CountAsync(),

                    TotalPrestamos = await _context.Prestamos.CountAsync(),
                    PrestamosActivos = await _context.Prestamos
                        .CountAsync(p => p.Estado == "Prestado" || p.Estado == "Atrasado"),
                    PrestamosAtrasados = await _context.Prestamos
                        .Where(p => p.FechaDevolucionReal == null && p.FechaDevolucionEsperada < DateTime.Now)
                        .CountAsync(),

                    TotalMultasPendientes = await _context.Prestamos
                        .Where(p => p.MultaPorRetraso.HasValue && p.Estado != "Devuelto")
                        .SumAsync(p => (decimal?)p.MultaPorRetraso) ?? 0,

                    TotalAutores = await _context.Autores.CountAsync(a => a.Activo),
                    TotalCategorias = await _context.Categorias.CountAsync(c => c.Activo)
                };

                return estadisticas;
            }
            catch (Exception ex)
            {
                // En caso de error, devolver estadísticas en cero
                return new EstadisticasGenerales();
            }
        }

        public async Task<List<LibroMasPrestado>> ObtenerLibrosMasPrestadosAsync(int cantidad = 10)
        {
            try
            {
                var libros = await _context.Prestamos
                    .Where(p => p.Libro != null && p.Libro.Autor != null)
                    .GroupBy(p => new
                    {
                        p.LibroId,
                        Titulo = p.Libro!.Titulo,
                        ISBN = p.Libro.ISBN,
                        AutorNombre = p.Libro.Autor!.Nombre,
                        AutorApellido = p.Libro.Autor.Apellido
                    })
                    .Select(g => new LibroMasPrestado
                    {
                        LibroId = g.Key.LibroId,
                        Titulo = g.Key.Titulo,
                        ISBN = g.Key.ISBN,
                        Autor = g.Key.AutorNombre + " " + g.Key.AutorApellido,
                        CantidadPrestamos = g.Count()
                    })
                    .OrderByDescending(l => l.CantidadPrestamos)
                    .Take(cantidad)
                    .ToListAsync();

                return libros;
            }
            catch
            {
                return new List<LibroMasPrestado>();
            }
        }

        public async Task<List<EstudianteMasPrestamos>> ObtenerEstudiantesConMasPrestamosAsync(int cantidad = 10)
        {
            try
            {
                var estudiantes = await _context.Prestamos
                    .Where(p => p.Estudiante != null)
                    .GroupBy(p => new
                    {
                        p.EstudianteId,
                        Nombre = p.Estudiante!.Nombre,
                        Apellido = p.Estudiante.Apellido,
                        Matricula = p.Estudiante.Matricula
                    })
                    .Select(g => new EstudianteMasPrestamos
                    {
                        EstudianteId = g.Key.EstudianteId,
                        NombreCompleto = g.Key.Nombre + " " + g.Key.Apellido,
                        Matricula = g.Key.Matricula,
                        CantidadPrestamos = g.Count(),
                        PrestamosActivos = g.Count(p => p.Estado == "Prestado" || p.Estado == "Atrasado")
                    })
                    .OrderByDescending(e => e.CantidadPrestamos)
                    .Take(cantidad)
                    .ToListAsync();

                return estudiantes;
            }
            catch
            {
                return new List<EstudianteMasPrestamos>();
            }
        }

        public async Task<List<PrestamosPorMes>> ObtenerPrestamosPorMesAsync(int meses = 6)
        {
            try
            {
                var fechaInicio = DateTime.Now.AddMonths(-meses);
                var culture = new CultureInfo("es-ES");

                var prestamos = await _context.Prestamos
                    .Where(p => p.FechaPrestamo >= fechaInicio)
                    .ToListAsync();

                var prestamosPorMes = prestamos
                    .GroupBy(p => new { p.FechaPrestamo.Year, p.FechaPrestamo.Month })
                    .Select(g => new PrestamosPorMes
                    {
                        Año = g.Key.Year,
                        Mes = g.Key.Month,
                        NombreMes = culture.DateTimeFormat.GetMonthName(g.Key.Month),
                        CantidadPrestamos = g.Count(),
                        CantidadDevoluciones = g.Count(p => p.Estado == "Devuelto")
                    })
                    .OrderBy(p => p.Año)
                    .ThenBy(p => p.Mes)
                    .ToList();

                return prestamosPorMes;
            }
            catch
            {
                return new List<PrestamosPorMes>();
            }
        }

        public async Task<List<CategoriaPopular>> ObtenerCategoriasMasPopularesAsync()
        {
            try
            {
                var libros = await _context.Libros
                    .Include(l => l.Categoria)
                    .Include(l => l.Prestamos)
                    .Where(l => l.Activo && l.Categoria != null)
                    .ToListAsync();

                var categorias = libros
                    .GroupBy(l => l.Categoria!.Nombre)
                    .Select(g => new CategoriaPopular
                    {
                        Categoria = g.Key,
                        CantidadLibros = g.Count(),
                        CantidadPrestamos = g.Sum(l => l.Prestamos?.Count ?? 0)
                    })
                    .OrderByDescending(c => c.CantidadPrestamos)
                    .ToList();

                return categorias;
            }
            catch
            {
                return new List<CategoriaPopular>();
            }
        }

        public async Task<List<Prestamo>> ObtenerPrestamosRecientesAsync(int cantidad = 10)
        {
            try
            {
                return await _context.Prestamos
                    .Include(p => p.Libro)
                        .ThenInclude(l => l!.Autor)
                    .Include(p => p.Estudiante)
                    .OrderByDescending(p => p.FechaPrestamo)
                    .Take(cantidad)
                    .ToListAsync();
            }
            catch
            {
                return new List<Prestamo>();
            }
        }

        public async Task<List<Prestamo>> ObtenerPrestamosProximosAVencerAsync(int dias = 3)
        {
            try
            {
                var fechaLimite = DateTime.Now.Date.AddDays(dias);
                var hoy = DateTime.Now.Date;

                return await _context.Prestamos
                    .Include(p => p.Libro)
                        .ThenInclude(l => l!.Autor)
                    .Include(p => p.Estudiante)
                    .Where(p => p.FechaDevolucionReal == null &&
                               p.FechaDevolucionEsperada.Date <= fechaLimite &&
                               p.FechaDevolucionEsperada.Date >= hoy)
                    .OrderBy(p => p.FechaDevolucionEsperada)
                    .ToListAsync();
            }
            catch
            {
                return new List<Prestamo>();
            }
        }
    }
}