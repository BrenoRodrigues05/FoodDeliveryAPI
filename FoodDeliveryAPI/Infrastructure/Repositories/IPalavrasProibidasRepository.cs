using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public interface IPalavrasProibidasRepository
    {
        Task<IEnumerable<PalavraProibida>> ObterPalavrasAsync();
    }
}
