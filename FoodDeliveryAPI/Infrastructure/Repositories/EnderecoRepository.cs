using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly FoodDeliveryAPIContext _context;
        private readonly ILogger<EnderecoRepository> _logger;

        public EnderecoRepository(FoodDeliveryAPIContext context, ILogger<EnderecoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }   

        public async Task<Endereco> CreateEnderecoAsync(Endereco endereco)
        {
            var novoEndereco = await _context.Enderecos.AddAsync(endereco);
            _logger.LogInformation("Novo endereço criado com ID: {EnderecoId}", novoEndereco.Entity.Id);
            return novoEndereco.Entity;
        }

        public async Task<Endereco?> GetEnderecoByIdAsync(int id)
        {
           var buscarEndereco = await _context.Enderecos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            _logger.LogInformation("Busca por endereço com ID: {EnderecoId} - Encontrado: {Encontrado}", id, buscarEndereco != null);
            return buscarEndereco;
        }

        public async Task<Endereco> UpdateEnderecoAsync(Endereco endereco)
        {
            var enderecoExistente = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == endereco.Id);
            if (enderecoExistente == null)
                throw new KeyNotFoundException();
            enderecoExistente.Nome = endereco.Nome;
            enderecoExistente.Rua = endereco.Rua;
            _logger.LogInformation("Endereço atualizado com ID: {EnderecoId}", enderecoExistente.Id);
            return enderecoExistente;
        }

        public async Task<bool> DeleteEnderecoAsync(int id)
        {
            var enderecoExistente = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == id);
            if (enderecoExistente == null)
            {
                _logger.LogWarning("Tentativa de deletar endereço com ID: {EnderecoId} - Não encontrado", id);
                return false;
            }
            _context.Enderecos.Remove(enderecoExistente);   
            _logger.LogInformation("Endereço deletado com ID: {EnderecoId}", id);
            return true;
        }

        public async Task<Endereco?> GetEnderecoByClienteIdAsync(int clienteId)
        {
           var busca = await _context.Enderecos.AsNoTracking().FirstOrDefaultAsync(e => e.ClienteId == clienteId);
            _logger.LogInformation("Busca por endereço com Cliente ID: {ClienteId} - Encontrado: {Encontrado}", clienteId, busca != null);
            return busca;
        }
    }
}
