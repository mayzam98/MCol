using MCol.BLL.Controller;
using MCol.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCol.Web.Controllers
{
    [ServiceFilter(typeof(JwtAuthenticationFilter))]

    public class DashboardController : Controller
    {
        private readonly SecurityController _securityController;

        public DashboardController(SecurityController securityController)
        {
            _securityController = securityController;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var username = User.Identity.Name;
            //var token = Request.Cookies["Token"]; // Obtener el token de las cookies

            //if (string.IsNullOrEmpty(token) || !_securityController.JwtCurrentUser(username, token))
            //{
            //    return RedirectToAction("Login", "Account"); // Redirigir a Login si no está autenticado
            //}

            var permissions = _securityController.GetUserPermissions(username, "Dashboard");
            if (permissions.Any(p => p.Acceso))
            {
                return View(); // Renderiza la vista "Index" del controlador "Dashboard"
            }

            return Forbid(); // Retorna un 403 si el usuario no tiene los permisos necesarios
        }
    }
}
