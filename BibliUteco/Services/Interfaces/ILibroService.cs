using BibliUteco.Models;

namespace BibliUteco.Services.Interfaces
{
    public interface ILibroService
    {
        Task<List<Libro>> ObtenerTodosAsync();
        Task<List<Libro>> ObtenerActivosAsync();
        Task<List<Libro>> ObtenerDisponiblesAsync();
        Task<Libro?> ObtenerPorIdAsync(int id);
        Task<Libro?> ObtenerPorISBNAsync(string isbn);
        Task<List<Libro>> BuscarAsync(string termino);
        Task<List<Libro>> ObtenerPorCategoriaAsync(int categoriaId);
        Task<List<Libro>> ObtenerPorAutorAsync(int autorId);
        Task<bool> CrearAsync(Libro libro);
        Task<bool> ActualizarAsync(Libro libro);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteISBNAsync(string isbn);
        Task<int> ContarTotalAsync();
        Task<int> ContarDisponiblesAsync();
        Task<bool> ActualizarDisponibilidadAsync(int libroId, int cantidad);
    }
}