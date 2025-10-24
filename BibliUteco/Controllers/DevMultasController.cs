using System.Linq;
using System.Threading.Tasks;
using BibliUteco.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliUteco.Controllers
{
    [ApiController]
    [Route("api/dev")]
    public class DevMultasController : ControllerBase
    {
        private readonly IMultaService _multaService;
        public DevMultasController(IMultaService multaService)
        {
            _multaService = multaService;
        }

        // GET api/dev/multas?pendientes=true
        [HttpGet("multas")]
        public async Task<IActionResult> GetMultas([FromQuery] bool pendientes = false)
        {
            var multas = pendientes
                ? await _multaService.ObtenerPendientesAsync()
                : await _multaService.ObtenerTodasAsync();

            var dto = multas.Select(m => new
            {
                m.MultaId,
                m.PrestamoId,
                Estudiante = m.Prestamo?.Estudiante?.NombreCompleto,
                Libro = m.Prestamo?.Libro?.Titulo,
                m.DiasRetraso,
                Monto = m.Monto,
                Estado = m.Estado,
                FechaGenerada = m.FechaGenerada,
                MetodoPago = m.MetodoPago,
                FechaPago = m.FechaPago
            });

            return Ok(dto);
        }
    }
}