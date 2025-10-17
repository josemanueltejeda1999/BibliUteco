using BibliUteco.Models;

namespace BibliUteco.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<EstadisticasGenerales> ObtenerEstadisticasGeneralesAsync();
        Task<List<LibroMasPrestado>> ObtenerLibrosMasPrestadosAsync(int cantidad = 10);
        Task<List<EstudianteMasPrestamos>> ObtenerEstudiantesConMasPrestamosAsync(int cantidad = 10);
        Task<List<PrestamosPorMes>> ObtenerPrestamosPorMesAsync(int meses = 6);
        Task<List<CategoriaPopular>> ObtenerCategoriasMasPopularesAsync();
        Task<List<Prestamo>> ObtenerPrestamosRecientesAsync(int cantidad = 10);
        Task<List<Prestamo>> ObtenerPrestamosProximosAVencerAsync(int dias = 3);
    }

    // Clases para las estadísticas
    public class EstadisticasGenerales
    {
        public int TotalLibros { get; set; }
        public int LibrosDisponibles { get; set; }
        public int LibrosPrestados { get; set; }
        public int TotalEstudiantes { get; set; }
        public int EstudiantesActivos { get; set; }
        public int TotalPrestamos { get; set; }
        public int PrestamosActivos { get; set; }
        public int PrestamosAtrasados { get; set; }
        public decimal TotalMultasPendientes { get; set; }
        public int TotalAutores { get; set; }
        public int TotalCategorias { get; set; }
    }

    public class LibroMasPrestado
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int CantidadPrestamos { get; set; }
    }

    public class EstudianteMasPrestamos
    {
        public int EstudianteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public int CantidadPrestamos { get; set; }
        public int PrestamosActivos { get; set; }
    }

    public class PrestamosPorMes
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int CantidadPrestamos { get; set; }
        public int CantidadDevoluciones { get; set; }
    }

    public class CategoriaPopular
    {
        public string Categoria { get; set; } = string.Empty;
        public int CantidadLibros { get; set; }
        public int CantidadPrestamos { get; set; }
    }
}