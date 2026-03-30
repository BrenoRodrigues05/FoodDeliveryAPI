using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Infrastructure.Repositories;

namespace FoodDeliveryAPI.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodDeliveryAPIContext _context;

        public IPedidoRepository PedidoRepository { get; }

        public UnitOfWork(FoodDeliveryAPIContext context, IPedidoRepository pedidoRepository)
        {
            _context = context;
            PedidoRepository = pedidoRepository;
        }

        public Task<int> CommitAsync() => _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
        
    }
}
