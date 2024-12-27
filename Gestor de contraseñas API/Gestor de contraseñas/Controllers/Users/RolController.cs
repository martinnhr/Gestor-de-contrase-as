using Gestor_de_contraseñas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Gestor_de_contraseñas.Controllers.Users
{
    [ApiController]
    [Route("api/[Controller]")]
    public class RolController : ControllerBase
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _singInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<MyRol> _roleManager;

        public RolController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IConfiguration configuration, RoleManager<MyRol> roleManager)
        {
            _userManager = userManager;
            _singInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost("createRol")]
        public async Task<IActionResult> CreateRol([FromBody] CreateRolModel model)
        {

        }
 
    }
}
