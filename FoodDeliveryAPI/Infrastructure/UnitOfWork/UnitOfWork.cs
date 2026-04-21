using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Infrastructure.Repositories;

namespace FoodDeliveryAPI.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodDeliveryAPIContext _context;

        public IPedidoRepository PedidoRepository { get; }
        public IEntregadorRepository EntregadorRepository { get; }

        public IClienteRepository ClienteRepository { get; }

        public IProdutoRepository ProdutoRepository { get; }

        public IEnderecoRepository EnderecoRepository { get; }

        public IPedidoItemRepository PedidoItemRepository { get; }
        public IPalavrasProibidasRepository PalavrasProibidasRepository { get; }

        public UnitOfWork(FoodDeliveryAPIContext context, IPedidoRepository pedidoRepository, IEntregadorRepository entregadorRepository, IClienteRepository clienteRepository, IProdutoRepository produtoRepository, IEnderecoRepository enderecoRepository, IPedidoItemRepository pedidoItemRepository, IPalavrasProibidasRepository palavrasProibidasRepository)
        {
            _context = context;
            PedidoRepository = pedidoRepository;
            EntregadorRepository = entregadorRepository;
            ClienteRepository = clienteRepository;
            ProdutoRepository = produtoRepository;
            EnderecoRepository = enderecoRepository;
            PedidoItemRepository = pedidoItemRepository;
            PalavrasProibidasRepository = palavrasProibidasRepository;
        }

        public Task<int> CommitAsync() => _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
        
    }
}
