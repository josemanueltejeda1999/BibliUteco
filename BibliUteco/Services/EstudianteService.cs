using BibliUteco.Data;
using BibliUteco.Models;
using BibliUteco.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliUteco.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly ApplicationDbContext _context;

        public EstudianteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Estudiante>> ObtenerTodosAsync()
        {
            return await _context.Estudiantes
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<List<Estudiante>> ObtenerActivosAsync()
        {
            return await _context.Estudiantes
                .Where(e => e.Activo)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<Estudiante?> ObtenerPorIdAsync(int id)
        {
            return await _context.Estudiantes
                .Include(e => e.Prestamos)
                .FirstOrDefaultAsync(e => e.EstudianteId == id);
        }

        public async Task<Estudiante?> ObtenerPorMatriculaAsync(string matricula)
        {
            return await _context.Estudiantes
                .Include(e => e.Prestamos)
                .FirstOrDefaultAsync(e => e.Matricula == matricula);
        }

        public async Task<Estudiante?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Estudiantes
                .Include(e => e.Prestamos)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<List<Estudiante>> BuscarAsync(string termino)
        {
            termino = termino.ToLower();
            return await _context.Estudiantes
                .Where(e => e.Activo &&
                    (e.Nombre.ToLower().Contains(termino) ||
                     e.Apellido.ToLower().Contains(termino) ||
                     e.Matricula.ToLower().Contains(termino) ||
                     e.Email.ToLower().Contains(termino) ||
                     (e.Carrera != null && e.Carrera.ToLower().Contains(termino))))
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<bool> CrearAsync(Estudiante estudiante)
        {
            try
            {
                estudiante.FechaRegistro = DateTime.Now;
                _context.Estudiantes.Add(estudiante);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(Estudiante estudiante)
        {
            try
            {
                _context.Estudiantes.Update(estudiante);
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
                var estudiante = await ObtenerPorIdAsync(id);
                if (estudiante == null) return false;

                // Verificar si tiene préstamos activos
                if (estudiante.Prestamos != null && estudiante.Prestamos.Any(p => p.Estado == "Prestado"))
                {
                    return false; // No se puede eliminar si tiene préstamos activos
                }

                // Eliminación lógica
                estudiante.Activo = false;
                await ActualizarAsync(estudiante);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Estudiantes.AnyAsync(e => e.EstudianteId == id);
        }

        public async Task<bool> ExisteMatriculaAsync(string matricula)
        {
            return await _context.Estudiantes.AnyAsync(e => e.Matricula == matricula);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Estudiantes.AnyAsync(e => e.Email == email);
        }

        public async Task<int> ContarTotalAsync()
        {
            return await _context.Estudiantes.CountAsync(e => e.Activo);
        }
    }
}