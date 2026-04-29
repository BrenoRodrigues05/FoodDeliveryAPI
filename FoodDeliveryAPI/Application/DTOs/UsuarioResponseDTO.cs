namespace FoodDeliveryAPI.Application.DTOs
{
    public class UsuarioResponseDTO
    {
        public string Email { get; set; }
        public string TipoUsuario { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
