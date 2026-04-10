using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IPedidoItemRepository
    {
        Task<PedidoItem?> GetByIdAsync(int id);
        Task<PedidoItem> CreateAsync (PedidoItem item);
        Task<PedidoItem> UpdateAsync (PedidoItem item);
        Task<bool> DeleteAsync (int id);

    }
}
