using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IEntregadorRepository
    {
        Task<IEnumerable<Entregador>> GetAllAsync();
        Task<Entregador> CreateAsync(Entregador entregador);
        Task<Entregador?> GetByIdAsync(int id);

        Task<bool> DeleteAsync (int id);

        Task <Entregador?> UpdateAsync (Entregador entregador);

        Task<Entregador?> GetByNameAsync(string nome);

        Task<Entregador?> GetPedidosEntregador(int id); 
    }
}
