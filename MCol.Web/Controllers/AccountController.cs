using MCol.BLL.Controller;
using MCol.DTO.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Common;
using System.Text;

namespace MCol.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SecurityController _securityController;

        public AccountController(SecurityController securityController)
        {
            _securityController = securityController;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("User") != null)
                return RedirectToAction("Index", "Dashboard");

            ViewBag.Url = Request.Query["url"];
            return View(new UserLoginDto());
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public  IActionResult Login(UserLoginDto loginDto)
        //{
        //    var TokenAutorizacion = "";
        //    try
        //    {

        //        var user = _securityController.Login(loginDto.Username, loginDto.Password);
        //        if (user != null)
        //        {
        //            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));
        //            HttpContext.Session.SetString("Permisos", JsonConvert.SerializeObject(_securityController.GetPermissions(user.Perfiles)));
        //            //var menu = _securityController.GetMenu(user.Perfiles);
        //            //HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(menu));
        //            TokenAutorizacion = user.TokenAutorizacion;

        //            // Guardar el token en las cookies
        //            //Response.Cookies.Append("Token", TokenAutorizacion);

        //            // Redirigir al Dashboard después de un login exitoso
        //            return RedirectToAction("Index", "Dashboard");
        //        }
        //        else
        //        {
        //            return RedirectToAction("Login", "Account");
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Json(TokenAutorizacion);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            try
            {
                var userdto = _securityController.Login(loginDto.Username, loginDto.Password);
                if (!string.IsNullOrEmpty(userdto.TokenAutorizacion))
                {
                    HttpContext.Session.SetString("UserToken", userdto.TokenAutorizacion);
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(loginDto));

                    // Save permissions and menu in session
                    var permissions = _securityController.GetPermissions(userdto.Perfiles);
                    HttpContext.Session.SetString("Permissions", JsonConvert.SerializeObject(permissions));

                    //var menu = _securityController.GetMenu(userdto.Perfiles);
                    //HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(menu));
                    Response.Cookies.Append("Token", userdto.TokenAutorizacion);

                    return RedirectToAction("Index", "Dashboard");
                }

                ViewData["Error"] = "Invalid login attempt.";
                return View("Login", loginDto);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "An error occurred during login.";
                return View("Login", loginDto);
            }
        }
        [HttpGet]
        public IActionResult Logout()
        {
            _securityController.ControlFinalSessionUser(HttpContext.Session.GetString("User"));
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
