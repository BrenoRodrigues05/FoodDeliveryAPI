using FoodDeliveryAPI.Infrastructure.Repositories;
using FoodDeliveryAPI.Infrastructure.UnitOfWork;

namespace FoodDeliveryAPI.Application.Services
{
    public class PalavrasProibidasService : IPalavrasProibidasService
    {
        private readonly ILogger<PalavrasProibidasService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPalavrasProibidasRepository _palavrasProibidasRepository;

        public PalavrasProibidasService(ILogger<PalavrasProibidasService> logger, IUnitOfWork unitOfWork, 
            IPalavrasProibidasRepository palavrasProibidasRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _palavrasProibidasRepository = palavrasProibidasRepository;
        }

        public async Task<bool> ContemPalavraProibida(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                _logger.LogWarning("Texto fornecido é nulo ou vazio.");
                return false;
            }

            _logger.LogInformation("Verificando palavras proibidas no texto.");

            var palavras = await _unitOfWork.PalavrasProibidasRepository.ObterPalavrasAsync();

            if (palavras == null || !palavras.Any())
            {
                _logger.LogInformation("Nenhuma palavra proibida cadastrada.");
                return false;
            }

            var contem = palavras.Any(p =>
                texto.Contains(p.Palavra, StringComparison.OrdinalIgnoreCase));

            if (contem)
                _logger.LogWarning("Texto contém palavras proibidas.");

            return contem;
        }
    }
}
