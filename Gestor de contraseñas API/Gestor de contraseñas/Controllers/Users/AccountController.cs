using Gestor_de_contraseñas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gestor_de_contraseñas.Controllers.Users.Models.Account;

namespace Gestor_de_contraseñas.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new MyUser
            {
                UserName = model.Name,
                Email = model.EmailAddress,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    result,
                    message = "User registered successfully"
                });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }

        }

        [HttpDelete("removeUser")]
        public async Task<IActionResult> RemoveUser([FromBody] RemoveUserModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null) 
            {
                return NotFound(new { message = "User not found"});
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "User removed" });

        }

        [HttpPatch("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null) 
            {
                return NotFound(new { message = "User not found"});
            }

            if(!string.IsNullOrEmpty(model.NewUserName))
            {
                user.UserName = model.NewUserName;
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                user.Email = model.Email;
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                user.PhoneNumber = model.PhoneNumber;
            }


            var res = await _userManager.UpdateAsync(user);

            return Ok(new { message = "User Updated" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user == null || !(await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                return Unauthorized(new { message = "Invalid user or password" });
            }

            var token = GenerateJwtTokenAsync(user);
            return Ok(new { token } );

        }



        private async Task<string> GenerateJwtTokenAsync(MyUser user)
        {

            // Obtener configuraciones de JWT
            var jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            var audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
            var expireMinutes = _configuration["Jwt:ExpireMinutes"] ?? throw new ArgumentNullException("Jwt:ExpireMinutes");

            var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

            // Definir las claims (información sobre el usuario en el token)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Obtener roles del usuario
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Crear la clave de firma
            var key = new SymmetricSecurityKey(jwtKeyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crear el descriptor del token
            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(expireMinutes)),
                signingCredentials: creds);

            // Generar el token
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}