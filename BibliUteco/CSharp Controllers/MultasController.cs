using BibliUteco.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliUteco.Controllers
{
    [ApiController]
    [Route("api/multas")]
    public class MultasController : ControllerBase
    {
        private readonly IMultaService _multaService;
        public MultasController(IMultaService multaService)
        {
            _multaService = multaService;
        }

        [HttpGet("{id}/comprobante")]
        public async Task<IActionResult> DescargarComprobante(int id)
        {
            var bytes = await _multaService.GenerarComprobantePdfAsync(id);
            if (bytes == null) return NotFound();

            var filename = $"Comprobante_Multa_{id}.pdf";
            return File(bytes, "application/pdf", filename);
        }
    }
}