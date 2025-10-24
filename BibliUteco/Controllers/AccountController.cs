using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BibliUteco.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        // GET /account/logout
        // Hace sign-out en una petición HTTP nueva (no dentro del circuito Blazor)
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Cerrar cookie de Identity (es la forma segura de evitar headers read-only en Blazor)
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                _logger.LogInformation("Usuario desconectado vía /account/logout");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error durante logout");
            }

            // Redirigir al inicio
            return LocalRedirect("/");
        }
    }
}