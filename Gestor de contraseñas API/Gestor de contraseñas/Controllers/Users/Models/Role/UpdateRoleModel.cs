using System.ComponentModel.DataAnnotations;

namespace Gestor_de_contraseñas.Controllers.Users.Models.Rol
{
    public class UpdateRoleModel
    {
        [Required]
        public string Name { get; set; }

        public string NewName { get; set; }
    }
}
