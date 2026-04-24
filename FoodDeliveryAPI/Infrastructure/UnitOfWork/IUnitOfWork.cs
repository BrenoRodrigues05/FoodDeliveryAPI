using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Infrastructure.Repositories;

namespace FoodDeliveryAPI.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
   
        public IPedidoRepository PedidoRepository { get; }
        public IEntregadorRepository EntregadorRepository { get; }

        public IPalavrasProibidasRepository PalavrasProibidasRepository { get; }

        public IClienteRepository ClienteRepository { get; }

        public IProdutoRepository ProdutoRepository { get; }

        public IEnderecoRepository EnderecoRepository { get; }


        Task<int> CommitAsync();
    }
}
