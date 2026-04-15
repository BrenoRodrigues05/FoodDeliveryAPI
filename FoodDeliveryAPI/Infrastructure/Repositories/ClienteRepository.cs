using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly FoodDeliveryAPIContext _context;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(FoodDeliveryAPIContext context, ILogger<ClienteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            var busca = await _context.Clientes.AsNoTracking().ToListAsync();
            _logger.LogInformation("Busca de clientes realizada com sucesso. Total de clientes encontrados: {Count}", busca.Count);
            return busca;
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            var cliente = await _context.Clientes.Include(p=> p.Pedidos).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            _logger.LogInformation("Busca de cliente por ID realizada. ID: {Id}, Cliente encontrado: {Found}", id, cliente != null);
            return cliente;
        }
        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            _logger.LogInformation("Criando novo cliente: {Nome}, Email: {Email}", cliente.Nome, cliente.Email);

            await _context.Clientes.AddAsync(cliente);

            return cliente;
        }

        public async Task<Cliente> UpdateAsync(Cliente cliente)
        {
           var clienteExistente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
            if (clienteExistente == null)
            {
                _logger.LogWarning("Cliente com ID {Id} não encontrado para atualização.", cliente.Id);
                throw new KeyNotFoundException($"Cliente com ID {cliente.Id} não encontrado.");
            }
            clienteExistente.Nome = cliente.Nome;
            clienteExistente.Email = cliente.Email;
            clienteExistente.Endereco = cliente.Endereco;
            _context.Clientes.Update(clienteExistente);
            _logger.LogInformation("Cliente com ID {Id} atualizado com sucesso.", cliente.Id);
            return clienteExistente;
        }

        public async Task <bool> DeleteAsync(int id)
        {
            var clienteExistente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (clienteExistente == null)
            {
                _logger.LogWarning("Cliente com ID {Id} não encontrado para exclusão.", id);
                return false;
            }
            _context.Clientes.Remove(clienteExistente);
            _logger.LogInformation("Cliente com ID {Id} excluído com sucesso.", id);
            return true;
        }

        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
            _logger.LogInformation("Busca de cliente por email realizada. Email: {Email}, Cliente encontrado: {Found}", email, cliente != null);
            return cliente;
        }


       
    }
}
