using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MCol.Web.Models;
using MCol.BLL.Controller;
using MCol.Web.Filters;

namespace MCol.Web.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly UsuariosControllerBLL _usuariosBLL;
    private readonly SecurityController _securityController;
    public HomeController(ILogger<HomeController> logger, UsuariosControllerBLL usuariosBLL, SecurityController securityController )
    :base(securityController)
    {
        _logger = logger;
        _usuariosBLL = usuariosBLL;
        _securityController = securityController;
    }
    [ServiceFilter(typeof(JwtAuthenticationFilter))]
    public IActionResult Index()
    {
        if (!ValidateSession())
        {
            return RedirectToAction("Login", "Account");
        }
        return View();
    }
    [ServiceFilter(typeof(JwtAuthenticationFilter))]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
