using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Infrastructure.Repositories;

namespace FoodDeliveryAPI.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodDeliveryAPIContext _context;

        public IPedidoRepository PedidoRepository { get; }
        public IEntregadorRepository EntregadorRepository { get; }

        public UnitOfWork(FoodDeliveryAPIContext context, IPedidoRepository pedidoRepository, IEntregadorRepository entregadorRepository)
        {
            _context = context;
            PedidoRepository = pedidoRepository;
            EntregadorRepository = entregadorRepository;
        }

        public Task<int> CommitAsync() => _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
        
    }
}
