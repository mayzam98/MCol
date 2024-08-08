using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using MCol.BLL.Controller;

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
            var authorizationHeader = request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
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
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;

                if (!_securityController.JwtCurrentUser(claimsIdentity.Name, token))
                {
                    throw new SecurityTokenValidationException("Invalid token.");
                }

                Thread.CurrentPrincipal = claimsPrincipal;
                context.HttpContext.User = claimsPrincipal;
            }
            catch (SecurityTokenValidationException)
            {
                context.Result = new UnauthorizedResult();
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
