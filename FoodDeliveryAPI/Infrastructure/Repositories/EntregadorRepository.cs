using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Domains.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class EntregadorRepository : IEntregadorRepository
    {
        private readonly FoodDeliveryAPIContext _context;
        private readonly ILogger<EntregadorRepository> _logger;

        public EntregadorRepository(FoodDeliveryAPIContext context, ILogger<EntregadorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Entregador>> GetAllAsync()
        {
            var entregadores = await _context.Entregadores.AsNoTracking().ToListAsync();
            _logger.LogInformation("Retrieved {Count} entregadores from the database.", entregadores.Count);
            return entregadores;
        }

        public async Task<Entregador?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Buscando entreagador com Id: {id}");
            var entregador = await  _context.Entregadores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);
            
            return entregador;
        }
        public async Task<Entregador> CreateAsync(Entregador entregador)
        {
            await _context.Entregadores.AddAsync(entregador);
            _logger.LogInformation("Criando perfil de entregador.");
            return entregador;
        }
         public async Task<Entregador?> UpdateAsync(Entregador entregador)
        {
           var buscarEntregador = await _context.Entregadores.FindAsync(entregador.Id);
            if (buscarEntregador == null)
            {
                _logger.LogWarning("Entregador with ID {Id} not found for update.", entregador.Id);
                return null;
            }
            buscarEntregador.Nome = entregador.Nome;
            buscarEntregador.Disponivel = entregador.Disponivel;
        
            return buscarEntregador;
        }
        public async Task<bool> DeleteAsync(int id)
        {
           var buscarEntregador = await _context.Entregadores.FindAsync(id);
            if (buscarEntregador == null)
            {
                _logger.LogWarning("Entregador with ID {Id} not found for deletion.", id);
                return false;
            }
            _context.Entregadores.Remove(buscarEntregador);
            _logger.LogInformation($"Entregador com id {id} deletado.");
            return true;

        }

        public Task<Entregador?> GetByNameAsync(string nome)
        {
            _logger.LogInformation($"Buscando entregador com nome: {nome}");
            var busca = _context.Entregadores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Nome == nome);

            return busca;
        }

        public async Task<Entregador?> GetPedidosEntregador(int id)
        {
           var entregador = await _context.Entregadores
                    .Include(e => e.Pedidos)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);
            if (entregador == null)
            {
                _logger.LogWarning("Entregador with ID {Id} not found when fetching orders.", id);
                return null;
            }
            _logger.LogInformation($"Retrieved entregador with ID {id} and their orders.");
            return entregador;
        }
    }
}
