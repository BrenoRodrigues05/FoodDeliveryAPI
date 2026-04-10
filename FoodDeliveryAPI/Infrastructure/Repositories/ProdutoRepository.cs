using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly FoodDeliveryAPIContext _context;
        private readonly ILogger<ProdutoRepository> _logger;

        public ProdutoRepository(ILogger<ProdutoRepository> logger, FoodDeliveryAPIContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Produto?> GetProdutoByIdAsync(int id)
        {
            var busca = await _context.Produtos.Include(p=> p.PedidoItens).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
           _logger.LogInformation("ProdutoRepository: GetProdutoByIdAsync - Produto buscado por ID: {ProdutoId}", id);
            return busca;
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
           var novoProduto = await _context.Produtos.AddAsync(produto);
            _logger.LogInformation("Criando produto: {Nome}, Preço: {Preco}", produto.Nome, produto.Preco);
            return novoProduto.Entity;
        }

        public async Task<Produto> UpdateProdutoAsync(Produto produto)
        {
            var produtoExistente = await _context.Produtos.FindAsync(produto.Id);
            if (produtoExistente == null)
            {
                _logger.LogWarning("ProdutoRepository: UpdateProdutoAsync - Produto não encontrado: {ProdutoId}", produto.Id);
                throw new KeyNotFoundException($"Produto com ID {produto.Id} não encontrado.");
            }
            produtoExistente.Nome = produto.Nome;
            produtoExistente.Preco = produto.Preco;
            _logger.LogInformation("ProdutoRepository: UpdateProdutoAsync - Produto atualizado: {ProdutoId}", produtoExistente.Id);
            return produtoExistente;
        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var busca = await _context.Produtos.FindAsync(id);
            if (busca == null)
            {
                _logger.LogWarning("ProdutoRepository: DeleteProdutoAsync - Produto não encontrado: {ProdutoId}", id);
                return false;
            }
            _context.Produtos.Remove(busca);
            _logger.LogInformation("ProdutoRepository: DeleteProdutoAsync - Produto deletado: {ProdutoId}", id);
            return true;
        }

        public async Task<Produto?> GetProdutoByNomeAsync(string nome)
        {
            var busca = await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Nome == nome);

            return busca;
        }

       
    }
}
