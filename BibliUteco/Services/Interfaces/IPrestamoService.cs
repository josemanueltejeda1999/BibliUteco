using BibliUteco.Models;

namespace BibliUteco.Services.Interfaces
{
    public interface IPrestamoService
    {
        Task<List<Prestamo>> ObtenerTodosAsync();
        Task<List<Prestamo>> ObtenerActivosAsync();
        Task<List<Prestamo>> ObtenerAtrasadosAsync();
        Task<Prestamo?> ObtenerPorIdAsync(int id);
        Task<List<Prestamo>> ObtenerPorEstudianteAsync(int estudianteId);
        Task<List<Prestamo>> ObtenerPorLibroAsync(int libroId);
        Task<bool> CrearPrestamoAsync(Prestamo prestamo);
        Task<bool> DevolverLibroAsync(int prestamoId, DateTime? fechaDevolucion = null);
        Task<bool> ActualizarAsync(Prestamo prestamo);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<int> ContarTotalAsync();
        Task<int> ContarActivosAsync();
        Task<int> ContarAtrasadosAsync();
        Task<decimal> CalcularMultaAsync(int prestamoId);
        Task ActualizarEstadosAtrasadosAsync();
    }
}