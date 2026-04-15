using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;
using System.Collections;

namespace FoodDeliveryAPI.Application.Services
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoResponseDTO>> GetPedidosAsync();
        Task<PedidoResponseDTO> GetPedidoByIdAsync(int id);
        Task<PedidoResponseDTO> CreatePedidoAsync(PedidoCreateDTO pedido, int clienteId);
        Task<bool> DeletePedidoAsync(int id);
        Task<PedidoResponseDTO> AtribuirEntregadorAsync(int pedidoId, int entregadorId);
        Task<PedidoResponseDTO> AtualizarStatusPedidoAsync(int pedidoId, string novoStatus);

    }
}
