using MCol.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCol.Web.Controllers
{
    //[ServiceFilter(typeof(JwtAuthenticationFilter))]
    //[Authorize]
    public class UsuariosController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
