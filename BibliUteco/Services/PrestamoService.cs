using BibliUteco.Data;
using BibliUteco.Models;
using BibliUteco.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // añadido

namespace BibliUteco.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMultaService _multaService;
        private readonly ILibroService _libroService; // añadido
        private readonly ILogger<PrestamoService> _logger; // añadido

        public PrestamoService(ApplicationDbContext context, IMultaService multaService, ILibroService libroService, ILogger<PrestamoService> logger)
        {
            _context = context;
            _multaService = multaService;
            _libroService = libroService; // si tu campo se llama diferente, mantén consistencia
            _logger = logger;
        }

        public async Task<List<Prestamo>> ObtenerTodosAsync()
        {
            return await _context.Prestamos
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Autor)
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Categoria)
                .Include(p => p.Estudiante)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();
        }

        public async Task<List<Prestamo>> ObtenerActivosAsync()
        {
            return await _context.Prestamos
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Autor)
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Categoria)
                .Include(p => p.Estudiante)
                .Where(p => p.Estado == "Prestado" || p.Estado == "Atrasado")
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();
        }

        public async Task<List<Prestamo>> ObtenerAtrasadosAsync()
        {
            var hoy = DateTime.Now.Date;
            return await _context.Prestamos
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Autor)
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Categoria)
                .Include(p => p.Estudiante)
                .Where(p => p.FechaDevolucionReal == null && p.FechaDevolucionEsperada.Date < hoy)
                .OrderBy(p => p.FechaDevolucionEsperada)
                .ToListAsync();
        }

        public async Task<Prestamo?> ObtenerPorIdAsync(int id)
        {
            return await _context.Prestamos
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Autor)
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Categoria)
                .Include(p => p.Estudiante)
                .FirstOrDefaultAsync(p => p.PrestamoId == id);
        }

        public async Task<List<Prestamo>> ObtenerPorEstudianteAsync(int estudianteId)
        {
            return await _context.Prestamos
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Autor)
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Categoria)
                .Include(p => p.Estudiante)
                .Where(p => p.EstudianteId == estudianteId)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();
        }

        public async Task<List<Prestamo>> ObtenerPorLibroAsync(int libroId)
        {
            return await _context.Prestamos
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Autor)
                .Include(p => p.Libro)
                    .ThenInclude(l => l!.Categoria)
                .Include(p => p.Estudiante)
                .Where(p => p.LibroId == libroId)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();
        }

        public async Task<bool> CrearPrestamoAsync(Prestamo prestamo)
        {
            try
            {
                // Verificar disponibilidad del libro
                var libro = await _libroService.ObtenerPorIdAsync(prestamo.LibroId);
                if (libro == null || libro.CantidadDisponible <= 0)
                    return false;

                // Crear el préstamo
                prestamo.FechaPrestamo = DateTime.Now;
                prestamo.FechaCreacion = DateTime.Now;
                prestamo.Estado = "Prestado";

                _context.Prestamos.Add(prestamo);

                // Actualizar disponibilidad del libro
                await _libroService.ActualizarDisponibilidadAsync(prestamo.LibroId, -1);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DevolverLibroAsync(int prestamoId, DateTime? fechaDevolucion = null)
        {
            try
            {
                var prestamo = await ObtenerPorIdAsync(prestamoId);
                if (prestamo == null || prestamo.Estado == "Devuelto")
                    return false;

                // Registrar devolución
                prestamo.FechaDevolucionReal = fechaDevolucion ?? DateTime.Now;
                prestamo.Estado = "Devuelto";

                // Calcular multa si hay retraso
                if (prestamo.FechaDevolucionReal > prestamo.FechaDevolucionEsperada)
                {
                    var diasRetraso = (prestamo.FechaDevolucionReal.Value - prestamo.FechaDevolucionEsperada).Days;
                    prestamo.MultaPorRetraso = diasRetraso * 50; // 50 pesos por día de retraso
                }

                _context.Prestamos.Update(prestamo);

                // Actualizar disponibilidad del libro
                await _libroService.ActualizarDisponibilidadAsync(prestamo.LibroId, 1);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(Prestamo prestamo)
        {
            try
            {
                _context.Prestamos.Update(prestamo);
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
                var prestamo = await ObtenerPorIdAsync(id);
                if (prestamo == null) return false;

                // Si el préstamo está activo, primero devolver el libro
                if (prestamo.Estado == "Prestado" || prestamo.Estado == "Atrasado")
                {
                    await DevolverLibroAsync(id);
                }

                _context.Prestamos.Remove(prestamo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Prestamos.AnyAsync(p => p.PrestamoId == id);
        }

        public async Task<int> ContarTotalAsync()
        {
            return await _context.Prestamos.CountAsync();
        }

        public async Task<int> ContarActivosAsync()
        {
            return await _context.Prestamos
                .CountAsync(p => p.Estado == "Prestado" || p.Estado == "Atrasado");
        }

        public async Task<int> ContarAtrasadosAsync()
        {
            var hoy = DateTime.Now.Date;
            return await _context.Prestamos
                .CountAsync(p => p.FechaDevolucionReal == null && p.FechaDevolucionEsperada.Date < hoy);
        }

        public async Task<decimal> CalcularMultaAsync(int prestamoId)
        {
            var prestamo = await ObtenerPorIdAsync(prestamoId);
            if (prestamo == null || prestamo.FechaDevolucionReal != null)
                return 0;

            if (DateTime.Now > prestamo.FechaDevolucionEsperada)
            {
                var diasRetraso = (DateTime.Now - prestamo.FechaDevolucionEsperada).Days;
                return diasRetraso * 50; // 50 pesos por día de retraso
            }

            return 0;
        }

        public async Task ActualizarEstadosAtrasadosAsync()
        {
            var hoy = DateTime.Now.Date;
            var prestamosAtrasados = await _context.Prestamos
                .Where(p => p.FechaDevolucionReal == null &&
                           p.FechaDevolucionEsperada.Date < hoy &&
                           p.Estado != "Atrasado")
                .ToListAsync();

            foreach (var prestamo in prestamosAtrasados)
            {
                prestamo.Estado = "Atrasado";
            }

            if (prestamosAtrasados.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RegistrarDevolucionAsync(int prestamoId)
        {
            var prestamo = await _context.Prestamos.FindAsync(prestamoId);
            if (prestamo == null) return false;

            prestamo.FechaDevolucionReal = DateTime.Now;

            var diasRetraso = (int)(prestamo.FechaDevolucionReal.Value - prestamo.FechaDevolucionEsperada).TotalDays;
            _logger.LogInformation("RegistrarDevolucionAsync: PrestamoId={PrestamoId} DiasRetraso={DiasRetraso}", prestamoId, diasRetraso);

            if (diasRetraso > 0)
            {
                prestamo.Estado = "Atrasado";
                // Generar multa (MultaService tiene logs)
                await _multaService.GenerarMultaAsync(prestamo, diasRetraso);
                _logger.LogInformation("Se solicitó generación de multa para PrestamoId={PrestamoId}", prestamoId);
            }
            else
            {
                prestamo.Estado = "Devuelto";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PuedeRealizarPrestamoAsync(int estudianteId)
        {
            return !await _multaService.TieneMultasPendientesAsync(estudianteId);
        }
    }
}