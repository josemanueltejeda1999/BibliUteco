using BibliUteco.Models;

namespace BibliUteco.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> ObtenerTodosAsync();
        Task<List<Categoria>> ObtenerActivosAsync();
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task<bool> CrearAsync(Categoria categoria);
        Task<bool> ActualizarAsync(Categoria categoria);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<int> ContarTotalAsync();
    }
}