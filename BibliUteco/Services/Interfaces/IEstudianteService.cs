using BibliUteco.Models;

namespace BibliUteco.Services.Interfaces
{
    public interface IEstudianteService
    {
        Task<List<Estudiante>> ObtenerTodosAsync();
        Task<List<Estudiante>> ObtenerActivosAsync();
        Task<Estudiante?> ObtenerPorIdAsync(int id);
        Task<Estudiante?> ObtenerPorMatriculaAsync(string matricula);
        Task<Estudiante?> ObtenerPorEmailAsync(string email);
        Task<List<Estudiante>> BuscarAsync(string termino);
        Task<bool> CrearAsync(Estudiante estudiante);
        Task<bool> ActualizarAsync(Estudiante estudiante);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteMatriculaAsync(string matricula);
        Task<bool> ExisteEmailAsync(string email);
        Task<int> ContarTotalAsync();
    }
}