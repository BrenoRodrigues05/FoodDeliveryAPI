using FoodDeliveryAPI.Application.DTOs;

namespace FoodDeliveryAPI.Application.Services
{
    public interface IEnderecoService
    {
        Task<EnderecoResponseDTO> ObterEnderecoDoCliente(int clienteId);
        Task<EnderecoResponseDTO> ObterEnderecoPorIdAsync(int id);
        Task<EnderecoResponseDTO> CriarEnderecoAsync(EnderecoCreateDTO endereco, int clienteId);
        Task<EnderecoResponseDTO> AtualizarEnderecoAsync(int enderecoId, EnderecoUpdateDTO endereco, int clienteId);

        Task<bool> ExcluirEnderecoAsync(int enderecoId, int clienteId);
    }
}
