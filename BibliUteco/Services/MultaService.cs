using System;
using System.IO;
using BibliUteco.Data;
using BibliUteco.Models;
using BibliUteco.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliUteco.Services
{
    public class MultaService : IMultaService
    {
        private readonly ApplicationDbContext _context;
        private readonly decimal _montoPorDia = 50.00M; // Monto en pesos por día de retraso
        private readonly ILogger<MultaService> _logger;

        public MultaService(ApplicationDbContext context, ILogger<MultaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Multa>> ObtenerTodasAsync()
        {
            return await _context.Multas
                .Include(m => m.Prestamo)
                    .ThenInclude(p => p!.Estudiante)
                .Include(m => m.Prestamo)
                    .ThenInclude(p => p!.Libro)
                .OrderByDescending(m => m.FechaGenerada)
                .ToListAsync();
        }

        public async Task<List<Multa>> ObtenerPendientesAsync()
        {
            // Normalizar comparación para evitar problemas de mayúsculas/espacios
            var pendientes = await _context.Multas
                .Include(m => m.Prestamo)
                    .ThenInclude(p => p!.Estudiante)
                .Include(m => m.Prestamo)
                    .ThenInclude(p => p!.Libro)
                .Where(m => m.Estado != null && m.Estado.Trim().ToLower() == "pendiente")
                .OrderByDescending(m => m.FechaGenerada)
                .ToListAsync();

            _logger.LogInformation("ObtenerPendientesAsync: encontrados {Count} multas pendientes", pendientes.Count);
            return pendientes;
        }

        public async Task<List<Multa>> ObtenerPorEstudianteAsync(int estudianteId)
        {
            return await _context.Multas
                .Include(m => m.Prestamo)
                    .ThenInclude(p => p!.Libro)
                .Where(m => m.Prestamo!.EstudianteId == estudianteId)
                .OrderByDescending(m => m.FechaGenerada)
                .ToListAsync();
        }

        public async Task<Multa?> ObtenerPorIdAsync(int multaId)
        {
            return await _context.Multas
                .Include(m => m.Prestamo)
                    .ThenInclude(p => p!.Estudiante)
                .Include(m => m.Prestamo)
                    .ThenInclude(p => p!.Libro)
                .FirstOrDefaultAsync(m => m.MultaId == multaId);
        }

        public async Task<Multa> GenerarMultaAsync(Prestamo prestamo, int diasRetraso)
        {
            try
            {
                _logger.LogInformation("GenerarMultaAsync inicio. PrestamoId={PrestamoId} DiasRetraso={DiasRetraso}", prestamo.PrestamoId, diasRetraso);

                var existente = await _context.Multas
                    .FirstOrDefaultAsync(m => m.PrestamoId == prestamo.PrestamoId && m.Estado != null && m.Estado.Trim().ToLower() == "pendiente");

                _logger.LogInformation("GenerarMultaAsync: existe multa pendiente para PrestamoId={PrestamoId}: {Existe}", prestamo.PrestamoId, existente != null);

                if (existente != null)
                {
                    return existente;
                }

                var multa = new Multa
                {
                    PrestamoId = prestamo.PrestamoId,
                    DiasRetraso = diasRetraso,
                    Monto = await CalcularMontoMultaAsync(diasRetraso),
                    Estado = "Pendiente",
                    FechaGenerada = DateTime.Now
                };

                _context.Multas.Add(multa);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Multa creada. MultaId={MultaId} PrestamoId={PrestamoId} Monto={Monto}", multa.MultaId, multa.PrestamoId, multa.Monto);

                return multa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando multa para PrestamoId={PrestamoId}", prestamo.PrestamoId);
                throw;
            }
        }

        public async Task<bool> PagarMultaAsync(int multaId, string metodoPago)
        {
            var multa = await _context.Multas.FindAsync(multaId);
            if (multa == null) return false;

            multa.Estado = "Pagada";
            multa.MetodoPago = metodoPago;
            multa.FechaPago = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("PagarMultaAsync: MultaId={MultaId} pagada por {Metodo}", multaId, metodoPago);
            return true;
        }

        public async Task<bool> TieneMultasPendientesAsync(int estudianteId)
        {
            var existe = await _context.Multas
                .AnyAsync(m => m.Prestamo!.EstudianteId == estudianteId &&
                               m.Estado != null && m.Estado.Trim().ToLower() == "pendiente");

            _logger.LogInformation("TieneMultasPendientesAsync: EstudianteId={EstudianteId} TienePendientes={Existe}", estudianteId, existe);
            return existe;
        }

        public Task<decimal> CalcularMontoMultaAsync(int diasRetraso)
        {
            return Task.FromResult(diasRetraso * _montoPorDia);
        }

        public async Task<MultasResumen> ObtenerResumenMultasAsync()
        {
            var multas = await _context.Multas.ToListAsync();

            return new MultasResumen
            {
                TotalMultas = multas.Count,
                MultasPagadas = multas.Count(m => string.Equals(m.Estado, "Pagada", StringComparison.OrdinalIgnoreCase)),
                MultasPendientes = multas.Count(m => m.Estado != null && m.Estado.Trim().ToLower() == "pendiente"),
                MontoTotalPendiente = multas
                    .Where(m => m.Estado != null && m.Estado.Trim().ToLower() == "pendiente")
                    .Sum(m => m.Monto),
                MontoTotalRecaudado = multas
                    .Where(m => string.Equals(m.Estado, "Pagada", StringComparison.OrdinalIgnoreCase))
                    .Sum(m => m.Monto)
            };
        }

        public async Task<byte[]?> GenerarComprobantePdfAsync(int multaId)
        {
            var multa = await ObtenerPorIdAsync(multaId);
            if (multa == null) return null;

            var estudiante = multa.Prestamo?.Estudiante?.NombreCompleto ?? "-";
            var libro = multa.Prestamo?.Libro?.Titulo ?? "-";
            var fechaPago = multa.FechaPago?.ToString("g") ?? "-";

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Element(header =>
                    {
                        header.Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("BibliUTECO").FontSize(18).Bold();
                                column.Item().Text("Comprobante de Pago de Multa").FontSize(14).SemiBold();
                            });

                            row.ConstantItem(100).AlignRight().Text($"Nº {multa.MultaId}").FontSize(12);
                        });
                    });

                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Fecha generado: {multa.FechaGenerada:dd/MM/yyyy HH:mm}");
                        column.Item().Text(string.Empty);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Campo").Bold();
                                header.Cell().Text("Valor").Bold();
                            });

                            table.Cell().Element(cell => cell.Text("Estudiante:"));
                            table.Cell().Element(cell => cell.Text(estudiante));

                            table.Cell().Element(cell => cell.Text("Libro:"));
                            table.Cell().Element(cell => cell.Text(libro));

                            table.Cell().Element(cell => cell.Text("Días de retraso:"));
                            table.Cell().Element(cell => cell.Text(multa.DiasRetraso.ToString()));

                            table.Cell().Element(cell => cell.Text("Monto:"));
                            table.Cell().Element(cell => cell.Text($"RD$ {multa.Monto:N2}"));

                            table.Cell().Element(cell => cell.Text("Método de pago:"));
                            table.Cell().Element(cell => cell.Text(multa.MetodoPago ?? "-"));

                            table.Cell().Element(cell => cell.Text("Fecha pago:"));
                            table.Cell().Element(cell => cell.Text(fechaPago));
                        });

                        column.Item().Text(string.Empty);
                        column.Item().Text("Gracias por su pago.").Bold();
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Comprobante generado por BibliUTECO");
                    });
                });
            });

            var pdfBytes = documento.GeneratePdf();

            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var dir = Path.Combine(webRoot, "comprobantes");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var filename = $"Comprobante_Multa_{multa.MultaId}.pdf";
            var fullpath = Path.Combine(dir, filename);
            await File.WriteAllBytesAsync(fullpath, pdfBytes);

            return pdfBytes;
        }
    }
}