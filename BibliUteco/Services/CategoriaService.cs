using BibliUteco.Data;
using BibliUteco.Models;
using BibliUteco.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliUteco.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;

        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> ObtenerTodosAsync()
        {
            return await _context.Categorias
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<List<Categoria>> ObtenerActivosAsync()
        {
            return await _context.Categorias
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias
                .Include(c => c.Libros)
                .FirstOrDefaultAsync(c => c.CategoriaId == id);
        }

        public async Task<bool> CrearAsync(Categoria categoria)
        {
            try
            {
                categoria.FechaCreacion = DateTime.Now;
                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(Categoria categoria)
        {
            try
            {
                _context.Categorias.Update(categoria);
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
                var categoria = await ObtenerPorIdAsync(id);
                if (categoria == null) return false;

                // Verificar si tiene libros asociados
                if (categoria.Libros != null && categoria.Libros.Any())
                {
                    // Eliminación lógica
                    categoria.Activo = false;
                    await ActualizarAsync(categoria);
                }
                else
                {
                    // Eliminación física
                    _context.Categorias.Remove(categoria);
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
            return await _context.Categorias.AnyAsync(c => c.CategoriaId == id);
        }

        public async Task<int> ContarTotalAsync()
        {
            return await _context.Categorias.CountAsync(c => c.Activo);
        }
    }
}