using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IProdutoRepository
    {
        Task<Produto?> GetProdutoByIdAsync(int id);
        Task<IEnumerable<Produto>> GetProdutosByNomeAsync(string nome);   

        Task<IEnumerable<Produto>> GetProdutosByPrecoAsync(decimal preco);

        Task<IEnumerable<Produto>> GetDisponiveisAsync();
        Task<Produto> CreateProdutoAsync(Produto produto);
        Task<Produto> UpdateProdutoAsync (Produto produto);
        Task<bool> DeleteProdutoAsync(int id);
    }
}
