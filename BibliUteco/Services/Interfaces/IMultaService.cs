using BibliUteco.Models;

namespace BibliUteco.Services.Interfaces
{
    public interface IMultaService
    {
        Task<List<Multa>> ObtenerTodasAsync();
        Task<List<Multa>> ObtenerPendientesAsync();
        Task<List<Multa>> ObtenerPorEstudianteAsync(int estudianteId);
        Task<Multa?> ObtenerPorIdAsync(int multaId);
        Task<Multa> GenerarMultaAsync(Prestamo prestamo, int diasRetraso);
        Task<bool> PagarMultaAsync(int multaId, string metodoPago);
        Task<bool> TieneMultasPendientesAsync(int estudianteId);
        Task<decimal> CalcularMontoMultaAsync(int diasRetraso);
        Task<MultasResumen> ObtenerResumenMultasAsync();
        Task<byte[]?> GenerarComprobantePdfAsync(int multaId);
    }

    public class MultasResumen
    {
        public int TotalMultas { get; set; }
        public int MultasPagadas { get; set; }
        public int MultasPendientes { get; set; }
        public decimal MontoTotalPendiente { get; set; }
        public decimal MontoTotalRecaudado { get; set; }
    }
}