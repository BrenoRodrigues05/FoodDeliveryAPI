using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Application.Services
{
    public interface IEntregadorService
    {
        Task<IEnumerable<Entregador>> GetEntregadoresAsync();
        Task<Entregador> GetEntregadorByIdAsync(int id);
        Task<Entregador> CreateEntregadorAsync(Entregador entregador);
        Task<bool> DeleteEntregadorAsync(int id);
        Task<Entregador> AtualizarDisponibilidadeEntregador(int entregadorId, string novaDisponibilidade);

    }
}
