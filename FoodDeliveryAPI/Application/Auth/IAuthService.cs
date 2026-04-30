using FoodDeliveryAPI.Application.DTOs;

namespace FoodDeliveryAPI.Application.Auth
{
    public interface IAuthService
    {
        Task<UsuarioResponseDTO> RegisterAsync(UsuarioCreateDTO usuarioCreateDTO);

        Task<UsuarioResponseDTO> LoginAsync(UsuarioLoginDTO usuarioLoginDTO);
    }
}
