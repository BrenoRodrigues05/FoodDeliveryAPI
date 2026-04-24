using FoodDeliveryAPI.Application.DTOs;

namespace FoodDeliveryAPI.Application.Services
{
    public interface IProdutoService
    {
      
        Task<IEnumerable<ProdutoResponseDTO>> GetProdutosByNomeAsync(string nome);

        Task<IEnumerable<ProdutoResponseDTO>> GetProdutosByPreco(decimal preco);

        Task<IEnumerable<ProdutoResponseDTO>> GetDisponiveisProdutosAsync();
        Task<ProdutoResponseDTO> CreateProdutoAsync(ProdutoCreateDTO produto);

        Task<ProdutoResponseDTO> GetProdutoByIdAsync(int id);

        Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoUpdateDTO produto);

        Task <bool> DeleteProdutoByIdAsync(int id);
    }
}
