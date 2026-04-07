using FoodDeliveryAPI.Domains.Entities;
using System.Collections;

namespace FoodDeliveryAPI.Application.Services
{
    public interface IPedidoService
    {
        Task<IEnumerable<Pedido>> GetPedidosAsync();
        Task<Pedido> GetPedidoByIdAsync(int id);
        Task<Pedido> CreatePedidoAsync(Pedido pedido);
        Task<bool> DeletePedidoAsync(int id);
        Task<Pedido> AtribuirEntregador(int pedidoId, int entregadorId);
        Task<Pedido> AtualizarStatusPedido(int pedidoId, string novoStatus);

    }
}
