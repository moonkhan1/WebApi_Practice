using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi_Practice.Models;

namespace WebApi_Practice.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly List<User> _users = new ()
        {
            new User
            {
                Id = 1, Name = "Test1", Surname = "Test1", Username = "test1", Password = "test1",
                Roles = new List<string>()
                {
                    "Admin"
                }
            },
            new User 
            {
                Id = 2, Name = "Test2", Surname = "Test2", Username = "test2", Password = "test2",
                Roles = new List<string>()
                {
                    "User"
                }
            },
            new User 
            {
                Id = 3, Name = "Test3", Surname = "Test3", Username = "test3", Password = "test3",
                Roles = new List < string >() 
                { 
                    "User" 
                }
            },
            new User
            {
                Id = 4, Name = "Test4", Surname = "Test4", Username = "test4", Password = "test4",
                Roles = new List < string >()
                {
                    "User",
                    "Admin"
                }
            },
            new User
            {
                Id = 5, Name = "Test5", Surname = "Test5", Username = "test5", Password = "test5"
            }
        };

        [HttpPost]
        public ActionResult<LoginResultModel> Login(LoginModel loginModel)
        {
            var user = _users.FirstOrDefault(u => u.Username ==  loginModel.Username && u.Password == loginModel.Password);

            if(user == null)
            {
                return NotFound(new
                {
                    message = "Username or Password is invalid"
                });
            }
            var token = GenerateJwtToken(user);

            return Ok(new LoginResultModel
            {
                UserId = user.Id,
                AuthToken = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 1 hours
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SigningKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };
            if(user.Roles.Count > 0)
            {
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "example.com",
                Audience = "example.com",
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set;}
        public string Password { get; set; }
        public string Username { get; set; }
        public List<string> Roles{ get; set;} = new();
    }
}
