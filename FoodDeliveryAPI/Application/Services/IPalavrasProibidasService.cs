namespace FoodDeliveryAPI.Application.Services
{
    public interface IPalavrasProibidasService
    {
        Task<bool> ContemPalavraProibida(string texto);
    }
}
