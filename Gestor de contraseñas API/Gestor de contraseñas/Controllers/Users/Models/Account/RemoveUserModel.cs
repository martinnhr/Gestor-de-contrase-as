using System.ComponentModel.DataAnnotations;

namespace Gestor_de_contraseñas.Controllers.Users.Models.Account
{
    public class RemoveUserModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
