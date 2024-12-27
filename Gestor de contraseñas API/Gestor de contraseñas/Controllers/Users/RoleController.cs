using Gestor_de_contraseñas.Controllers.Users.Models.Rol;
using Gestor_de_contraseñas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gestor_de_contraseñas.Controllers.Users
{
    [ApiController]
    [Route("api/[Controller]")]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<MyRole> _roleManager;

        public RoleController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IConfiguration configuration, RoleManager<MyRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new MyRole
            {
                Name = model.Name
            };

            try
            {
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        result,
                        message = "Role created successfully"
                    });
                }

                return BadRequest(new
                {
                    Errors = result.Errors.Select(e => e.Description)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete("removeRole")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var role = await _roleManager.FindByNameAsync(model.Name);
                if (role == null)
                {
                    return NotFound(new { message = "Role not found" });
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { message = "Role removed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        [HttpPatch("updateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var role = await _roleManager.FindByNameAsync(model.Name);
                if (role == null)
                {
                    return NotFound(new { message = "Role not found" });
                }

                if (!string.IsNullOrEmpty(model.NewName))
                {
                    role.Name = model.NewName;
                }

                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { message = "Role updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(model.userName);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var role = await _roleManager.FindByNameAsync(model.roleName);
                if (role == null)
                {
                    return NotFound(new { message = "Role not found" });
                }

                var result = await _userManager.AddToRoleAsync(user, model.roleName);
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { message = "Role successfully assigned" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }
    }
}
