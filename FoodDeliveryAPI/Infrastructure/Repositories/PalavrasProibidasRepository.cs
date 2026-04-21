using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class PalavrasProibidasRepository : IPalavrasProibidasRepository
    {
        private readonly ILogger<PalavrasProibidasRepository> _logger;
        private readonly FoodDeliveryAPIContext _context;
        public PalavrasProibidasRepository(ILogger<PalavrasProibidasRepository> logger, FoodDeliveryAPIContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<PalavraProibida>> ObterPalavrasAsync()
        {
            _logger.LogInformation("Obtendo palavras proibidas do repositório.");

            return await _context.PalavrasProibidas.AsNoTracking().ToListAsync();

        }
    }
}
