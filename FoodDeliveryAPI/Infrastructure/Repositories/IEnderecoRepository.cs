using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IEnderecoRepository
    {
        Task<Endereco?> GetEnderecoByIdAsync(int id);
        Task<Endereco> CreateEnderecoAsync(Endereco endereco);
        Task<Endereco> UpdateEnderecoAsync(Endereco endereco);
        Task<bool> DeleteEnderecoAsync(int id);
    }
}
