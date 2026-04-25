using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Domains.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly FoodDeliveryAPIContext _context;
        private readonly ILogger<PedidoRepository> _logger;
        public PedidoRepository(FoodDeliveryAPIContext context, ILogger<PedidoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todos os pedidos.");

            return await _context.Pedidos
                    .AsNoTracking()
                    .ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
           
            _logger.LogInformation("Buscando pedido com ID {Id}.", id);

            var busca = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Entregador)
                .Include(p => p.PedidoItens)
                    .ThenInclude(pi => pi.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            return busca;

        }
        public async Task<Pedido> AddAsync(Pedido pedido)
        {

            await _context.Pedidos.AddAsync(pedido);

            _logger.LogInformation("Pedido adicionado com ID {Id}.", pedido.Id);

            return pedido;
        }
        public async Task<bool> DeleteAsync(int id)
        {

            var affected = await _context.Pedidos
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();

            _logger.LogInformation("Pedido com ID {Id} excluído.", id);

            return affected > 0;
        }

       public async Task<Pedido> UpdateAsync(Pedido pedido)
        {
            var affected = await _context.Pedidos
                .Where(p => p.Id == pedido.Id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(p => p.Status, pedido.Status)
                    .SetProperty(p => p.EntregadorId, pedido.EntregadorId)
                );
            _logger.LogInformation("Pedido com ID {Id} atualizado.", pedido.Id);

            if (affected == 0)
                throw new Exception("Pedido não encontrado.");

            return pedido;
        }

        public async Task<IEnumerable<Pedido>> GetByClienteIdAsync(int clienteId)
        {
            var busca = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.ClienteId == clienteId)
                .Include(p => p.Cliente)
                .Include(p => p.Entregador)
                .Include(p => p.ValorTotal)
                .Include(p => p.PedidoItens)
                    .ThenInclude(pi => pi.Produto)
                .ToListAsync();

            return busca.ToList();
        }

        public async Task<Pedido> GetEmTransitoByEntregador(int entregadorId)
        {
            var busca = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.EntregadorId == entregadorId && p.Status == StatusPedido.EmTransito)
                .Include(p => p.Cliente)
                .Include(p => p.Entregador)
                .FirstOrDefaultAsync();

            if( busca == null)
            {
                _logger.LogInformation("Nenhum pedido em trânsito encontrado para o entregador com ID {Id}.", entregadorId);
                throw new KeyNotFoundException($"Nenhum pedido em trânsito encontrado para o entregador com ID {entregadorId}.");
            }

            return busca;
        }
    }
}
