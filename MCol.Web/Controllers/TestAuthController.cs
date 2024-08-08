using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MCol.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TestAuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("This is a public endpoint.");
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok("This is a protected endpoint.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok("This is an Admin-only endpoint.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin model)
        {
            var user = Authenticate(model);

            if (user != null)
            {
                var (token, expiration) = GenerateToken(user);
                return Ok(new
                {
                    token,
                    expiration,
                    token_type = "Bearer"
                });
            }

            return Unauthorized();
        }

        private User Authenticate(UserLogin model)
        {
            if (model.Username == "admin" && model.Password == "password")
            {
                return new User { Username = model.Username, Role = "Admin" };
            }

            if (model.Username == "user" && model.Password == "password")
            {
                return new User { Username = model.Username, Role = "User" };
            }

            return null;
        }

        private (string token, DateTime expiration) GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiration = DateTime.Now.AddMinutes(30);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return (tokenHandler.WriteToken(token), expiration);
        }
    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class User
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
