using System.ComponentModel.DataAnnotations;

namespace Gestor_de_contraseñas.Controllers.Users.Models.Rol
{
    public class AssignRoleModel
    {
        [Required]
        public String userName { get; set; }

        [Required]
        public String roleName { get; set; }
    }
}
