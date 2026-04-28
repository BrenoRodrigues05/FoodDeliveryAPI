using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Application.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteResponseDTO>> GetAllClientesAsync();
        Task<ClienteResponseDTO> GetClienteByIdAsync(int id);
        Task<ClienteResponseDTO> GetClienteByEmailAsync(string email);

        Task<ClienteResponseDTO> UpdateClienteAsync(int id, ClienteUpdateDTO cliente);

        Task<bool> DeleteClienteAsync(int id);

        Task<PedidoResponseDTO> BuscarPedidosClienteAsync(int clienteId);

        Task<EnderecoResponseDTO> BuscarEnderecoClienteAsync(int clienteId);

       

    }
}
