using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MCol.Web.Models;
using MCol.BLL.Controller;

namespace MCol.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UsuariosControllerBLL _usuariosBLL;

    public HomeController(ILogger<HomeController> logger, UsuariosControllerBLL usuariosBLL)
    {
        _logger = logger;
        _usuariosBLL = usuariosBLL;
    }

    public IActionResult Index()
    {  
        return View();
    }

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
