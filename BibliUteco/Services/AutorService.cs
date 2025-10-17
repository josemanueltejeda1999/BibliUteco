using BibliUteco.Data;
using BibliUteco.Models;
using BibliUteco.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliUteco.Services
{
    public class AutorService : IAutorService
    {
        private readonly ApplicationDbContext _context;

        public AutorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Autor>> ObtenerTodosAsync()
        {
            return await _context.Autores
                .OrderBy(a => a.Apellido)
                .ThenBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<List<Autor>> ObtenerActivosAsync()
        {
            return await _context.Autores
                .Where(a => a.Activo)
                .OrderBy(a => a.Apellido)
                .ThenBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<Autor?> ObtenerPorIdAsync(int id)
        {
            return await _context.Autores
                .Include(a => a.Libros)
                .FirstOrDefaultAsync(a => a.AutorId == id);
        }

        public async Task<bool> CrearAsync(Autor autor)
        {
            try
            {
                autor.FechaCreacion = DateTime.Now;
                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(Autor autor)
        {
            try
            {
                _context.Autores.Update(autor);
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
                var autor = await ObtenerPorIdAsync(id);
                if (autor == null) return false;

                // Verificar si tiene libros asociados
                if (autor.Libros != null && autor.Libros.Any())
                {
                    // Eliminación lógica
                    autor.Activo = false;
                    await ActualizarAsync(autor);
                }
                else
                {
                    // Eliminación física
                    _context.Autores.Remove(autor);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Autores.AnyAsync(a => a.AutorId == id);
        }

        public async Task<int> ContarTotalAsync()
        {
            return await _context.Autores.CountAsync(a => a.Activo);
        }
    }
}