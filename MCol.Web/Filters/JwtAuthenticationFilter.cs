using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using MCol.BLL.Controller;
using Microsoft.CodeAnalysis.Elfie.Model;
using System.IdentityModel.Tokens.Jwt;

namespace MCol.Web.Filters
{
    public class JwtAuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly SecurityController _securityController;

        public JwtAuthenticationFilter(IConfiguration configuration, SecurityController securityController)
        {
            _configuration = configuration;
            _securityController = securityController;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var cokie = request.Cookies["Token"] == null ? "": request.Cookies["Token"].ToString();
            if (!string.IsNullOrEmpty(cokie) )
            {
                request.Headers.Add("Authorization", "Bearer " + cokie);
            }

            var authorizationHeader = request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                //context.Result = new UnauthorizedResult();
                context.HttpContext.Session.Clear();
                context.Result = new RedirectResult("~/Account/Login");
                return;
            }

            var token = authorizationHeader.Substring(7);
            var secretKey = _configuration["Jwt:Key"];
            var audienceToken = _configuration["Jwt:Audience"];
            var issuerToken = _configuration["Jwt:Issuer"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidAudience = audienceToken,
                ValidIssuer = issuerToken,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey
            };

            try
            {

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                if (claimsIdentity == null || !claimsIdentity.Claims.Any())
                {
                    // Si no hay claims o algo está mal, retorna no autorizado
                    context.HttpContext.Session.Clear();
                    context.Result = new RedirectResult("~/Account/Login");
                    return;
                }
                if (!_securityController.JwtCurrentUser(claimsIdentity.Name, token))
                {
                    context.HttpContext.Session.Clear();
                    context.Result = new RedirectResult("~/Account/Login");
                    //throw new SecurityTokenValidationException("Invalid token.");

                    return;
                }
                if (securityToken is JwtSecurityToken jwtToken)
                {
                    var exp = jwtToken.ValidTo;
                    if (exp < DateTime.UtcNow)
                    {
                        context.HttpContext.Session.Clear();
                        context.Result = new RedirectResult("~/Account/Login");
                        return;
                    }
                }

                Thread.CurrentPrincipal = claimsPrincipal;
                context.HttpContext.User = claimsPrincipal;
            }
            catch (SecurityTokenValidationException)
            {
                //context.Result = new UnauthorizedResult();
                context.HttpContext.Session.Clear();
                context.Result = new RedirectResult("~/Account/Login");
                return;

            }
            catch (Exception)
            {
                //context.Result = new UnauthorizedResult();
                context.HttpContext.Session.Clear();
                context.Result = new RedirectResult("~/Account/Login");
                return;

            }
        }
    }
}
