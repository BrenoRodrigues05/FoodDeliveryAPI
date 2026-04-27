using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Application.DTOs
{
    public class UsuarioCreateDTO
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public string TipoUsuario { get; set; }
    }
}
