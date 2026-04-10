using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class PedidoItemRepository : IPedidoItemRepository
    {
        private readonly FoodDeliveryAPIContext _context;
        private readonly ILogger<PedidoItemRepository> _logger;

        public PedidoItemRepository(FoodDeliveryAPIContext context, ILogger<PedidoItemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PedidoItem?> GetByIdAsync(int id)
        {
            var busca = await _context.PedidoItens
                .AsNoTracking()
                .Include(pi => pi.Produto)
                .FirstOrDefaultAsync(pi => pi.Id == id);
            _logger.LogInformation("PedidoItem with ID {Id} retrieved successfully.", id);
            return busca;
        }
        public async Task<PedidoItem> CreateAsync(PedidoItem item)
        {
            var novoItem = _context.PedidoItens.Add(item);
            _logger.LogInformation("Criando novo PedidoItem para ProdutoId {ProdutoId}", item.ProdutoId);
            return novoItem.Entity;
        }

        public async Task<PedidoItem> UpdateAsync(PedidoItem item)
        {
            var itemExistente = await _context.PedidoItens.FindAsync(item.Id);
            if (itemExistente == null)
            {
                _logger.LogWarning("PedidoItem with ID {Id} not found for update.", item.Id);
                throw new KeyNotFoundException($"PedidoItem with ID {item.Id} not found.");
            }
            
            itemExistente.Quantidade = item.Quantidade;
            _logger.LogInformation("PedidoItem with ID {Id} updated successfully.", item.Id);
            return itemExistente;

        }
        public async Task<bool> DeleteAsync(int id)
        {
            var busca = await _context.PedidoItens.FindAsync(id);
            if (busca == null)
            {
                _logger.LogWarning("PedidoItem with ID {Id} not found for deletion.", id);
                return false;
            }
            _logger.LogInformation("PedidoItem with ID {Id} deleted successfully.", id);
            _context.PedidoItens.Remove(busca);
            return true;
        }

       
    }
}
