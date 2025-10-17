using BibliUteco.Models;

namespace BibliUteco.Services.Interfaces
{
    public interface IAutorService
    {
        Task<List<Autor>> ObtenerTodosAsync();
        Task<List<Autor>> ObtenerActivosAsync();
        Task<Autor?> ObtenerPorIdAsync(int id);
        Task<bool> CrearAsync(Autor autor);
        Task<bool> ActualizarAsync(Autor autor);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<int> ContarTotalAsync();
    }
}