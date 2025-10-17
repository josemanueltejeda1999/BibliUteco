using BibliUteco.Data;
using BibliUteco.Models;
using BibliUteco.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliUteco.Services
{
    public class LibroService : ILibroService
    {
        private readonly ApplicationDbContext _context;

        public LibroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Libro>> ObtenerTodosAsync()
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .OrderBy(l => l.Titulo)
                .ToListAsync();
        }

        public async Task<List<Libro>> ObtenerActivosAsync()
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Activo)
                .OrderBy(l => l.Titulo)
                .ToListAsync();
        }

        public async Task<List<Libro>> ObtenerDisponiblesAsync()
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Activo && l.CantidadDisponible > 0)
                .OrderBy(l => l.Titulo)
                .ToListAsync();
        }

        public async Task<Libro?> ObtenerPorIdAsync(int id)
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Include(l => l.Prestamos)
                .FirstOrDefaultAsync(l => l.LibroId == id);
        }

        public async Task<Libro?> ObtenerPorISBNAsync(string isbn)
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.ISBN == isbn);
        }

        public async Task<List<Libro>> BuscarAsync(string termino)
        {
            termino = termino.ToLower();
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.Activo &&
                    (l.Titulo.ToLower().Contains(termino) ||
                     l.ISBN.ToLower().Contains(termino) ||
                     l.Autor!.Nombre.ToLower().Contains(termino) ||
                     l.Autor!.Apellido.ToLower().Contains(termino) ||
                     l.Editorial.ToLower().Contains(termino)))
                .OrderBy(l => l.Titulo)
                .ToListAsync();
        }

        public async Task<List<Libro>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.CategoriaId == categoriaId && l.Activo)
                .OrderBy(l => l.Titulo)
                .ToListAsync();
        }

        public async Task<List<Libro>> ObtenerPorAutorAsync(int autorId)
        {
            return await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Categoria)
                .Where(l => l.AutorId == autorId && l.Activo)
                .OrderBy(l => l.Titulo)
                .ToListAsync();
        }

        public async Task<bool> CrearAsync(Libro libro)
        {
            try
            {
                libro.FechaCreacion = DateTime.Now;
                libro.CantidadDisponible = libro.CantidadTotal;
                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(Libro libro)
        {
            try
            {
                _context.Libros.Update(libro);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var libro = await ObtenerPorIdAsync(id);
                if (libro == null) return false;

                // Verificar si tiene préstamos activos
                if (libro.Prestamos != null && libro.Prestamos.Any(p => p.Estado == "Prestado"))
                {
                    return false; // No se puede eliminar si hay préstamos activos
                }

                // Eliminación lógica
                libro.Activo = false;
                await ActualizarAsync(libro);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Libros.AnyAsync(l => l.LibroId == id);
        }

        public async Task<bool> ExisteISBNAsync(string isbn)
        {
            return await _context.Libros.AnyAsync(l => l.ISBN == isbn);
        }

        public async Task<int> ContarTotalAsync()
        {
            return await _context.Libros.CountAsync(l => l.Activo);
        }

        public async Task<int> ContarDisponiblesAsync()
        {
            return await _context.Libros
                .Where(l => l.Activo && l.CantidadDisponible > 0)
                .CountAsync();
        }

        public async Task<bool> ActualizarDisponibilidadAsync(int libroId, int cantidad)
        {
            try
            {
                var libro = await _context.Libros.FindAsync(libroId);
                if (libro == null) return false;

                libro.CantidadDisponible += cantidad;

                // Validar que no sea negativo
                if (libro.CantidadDisponible < 0)
                    libro.CantidadDisponible = 0;

                // Validar que no supere el total
                if (libro.CantidadDisponible > libro.CantidadTotal)
                    libro.CantidadDisponible = libro.CantidadTotal;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}