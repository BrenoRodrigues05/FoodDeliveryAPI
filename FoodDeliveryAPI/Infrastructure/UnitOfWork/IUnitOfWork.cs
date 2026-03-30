using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Infrastructure.Repositories;

namespace FoodDeliveryAPI.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
   
        public IPedidoRepository PedidoRepository { get; }

        Task<int> CommitAsync();
    }
}
