using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> GetByEmailAsync(string email);

        Task<Usuario> AddAsync (Usuario usuario);

        Task<bool> DeleteAsync (int id);
    }
}
