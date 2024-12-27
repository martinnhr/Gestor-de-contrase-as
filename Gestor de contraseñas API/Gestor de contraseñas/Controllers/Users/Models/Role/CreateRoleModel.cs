using System.ComponentModel.DataAnnotations;

namespace Gestor_de_contraseñas.Controllers.Users.Models.Rol
{
    public class CreateRoleModel
    {
        [Required]
        public  string Name { get; set; }

    }
}
