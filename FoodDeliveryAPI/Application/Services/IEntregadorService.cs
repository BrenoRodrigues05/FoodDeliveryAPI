using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Application.Services
{
    public interface IEntregadorService
    {
        Task<IEnumerable<EntregadorResponseDTO>> GetEntregadoresAsync();
        Task<EntregadorResponseDTO> GetEntregadorByIdAsync(int id);
        Task<EntregadorResponseDTO> CreateEntregadorAsync(EntregadorCreateDTO entregador);
        Task<bool> DeleteEntregadorAsync(int id);
        Task<EntregadorResponseDTO> AtualizarDisponibilidadeEntregadorAsync(int entregadorId, bool novaDisponibilidade);

    }
}
