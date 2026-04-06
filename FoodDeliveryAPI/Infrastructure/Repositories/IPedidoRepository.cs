using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IPedidoRepository
    {
         Task<IEnumerable<Pedido>> GetAllAsync();
         Task<Pedido> GetByIdAsync(int id);
        Task<Pedido> AddAsync(Pedido pedido);
        Task<bool> DeleteAsync(int id);
        Task<Pedido> UpdateAsync(Pedido pedido);

    }
}
