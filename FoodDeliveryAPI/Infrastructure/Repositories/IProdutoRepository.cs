using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IProdutoRepository
    {
        Task<Produto?> GetProdutoByIdAsync(int id);
        Task<Produto?> GetProdutoByNomeAsync(string nome);   
        Task<Produto> CreateProdutoAsync(Produto produto);
        Task<Produto> UpdateProdutoAsync (Produto produto);
        Task<bool> DeleteProdutoAsync(int id);
    }
}
