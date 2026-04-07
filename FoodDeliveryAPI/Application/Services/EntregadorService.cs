using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Infrastructure.Repositories;
using FoodDeliveryAPI.Infrastructure.UnitOfWork;
using System.Globalization;
using System.Text;

namespace FoodDeliveryAPI.Application.Services
{
    public class EntregadorService : IEntregadorService
    {
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EntregadorService> _logger;

        public EntregadorService(IEntregadorRepository entregadorRepository, IPedidoRepository pedidoRepository, 
            IUnitOfWork unitOfWork, ILogger<EntregadorService> logger)
        {
            _entregadorRepository = entregadorRepository;
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Entregador>> GetEntregadoresAsync()
        {
            _logger.LogInformation("Recuperando lista de entregadores.");
            var entregadores = await _entregadorRepository.GetAllAsync();
            return entregadores;
        }

        public async Task<Entregador> GetEntregadorByIdAsync(int id)
        {
           if(id <= 0)
            {
                _logger.LogWarning("ID do entregador é inválido: {Id}", id);
                throw new ArgumentException("ID do entregador é inválido.", nameof(id));
            }

           var busca = await _entregadorRepository.GetByIdAsync(id);

            if(busca  == null)
            {
                _logger.LogWarning("Entregador não encontrado com ID: {Id}", id);
                throw new KeyNotFoundException($"Entregador com ID {id} não encontrado.");
            }

            _logger.LogInformation("Entregador encontrado: {Nome}", busca.Nome);
            return busca;

        }
        public async Task<Entregador> CreateEntregadorAsync(Entregador entregador)
        {
            if (entregador == null)
            {
                _logger.LogWarning("Entregador fornecido é nulo.");
                throw new ArgumentNullException(nameof(entregador), "Entregador não pode ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(entregador.Nome))
            {
                _logger.LogWarning("Nome do entregador é inválido.");
                throw new ArgumentException("Nome do entregador é obrigatório.", nameof(entregador.Nome));
            }

           var buscaExistente = await _entregadorRepository.GetByNameAsync(entregador.Nome);
            if (buscaExistente != null)
            {
                _logger.LogWarning("Entregador com nome '{Nome}' já existe.", entregador.Nome);
                throw new InvalidOperationException($"Entregador com nome '{entregador.Nome}' já existe.");
            }
            var novoEntregador = new Entregador
            {
                Nome = entregador.Nome,
                Disponivel = entregador.Disponivel
            };
            await _entregadorRepository.CreateAsync(novoEntregador);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Novo entregador criado: {Nome}", novoEntregador.Nome);
            return novoEntregador;

        }

        public async Task<bool> DeleteEntregadorAsync(int id)
        {
           if (id <= 0)
            {
                _logger.LogWarning("ID do entregador é inválido: {Id}", id);
                throw new ArgumentException("ID do entregador é inválido.", nameof(id));
            }

           var busca = await _entregadorRepository.GetByIdAsync(id);
            if (busca == null)
            {
                _logger.LogWarning("Entregador não encontrado com ID: {Id}", id);
                throw new KeyNotFoundException($"Entregador com ID {id} não encontrado.");
            }
            await _entregadorRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Entregador deletado: {Nome}", busca.Nome);
            return true;
        }

        public async Task<Entregador> AtualizarDisponibilidadeEntregador(int entregadorId, string novaDisponibilidade)
        {
            if (entregadorId <= 0)
            {
                _logger.LogWarning("ID do entregador é inválido: {Id}", entregadorId);
                throw new ArgumentException("ID do entregador é inválido.", nameof(entregadorId));
            }

            if (string.IsNullOrWhiteSpace(novaDisponibilidade))
            {
                _logger.LogWarning("Nova disponibilidade é inválida.");
                throw new ArgumentException("Nova disponibilidade é obrigatória.", nameof(novaDisponibilidade));
            }

            var buscaEntregador = await _entregadorRepository.GetByIdAsync(entregadorId);

            if (buscaEntregador == null)
            {
                _logger.LogWarning("Entregador não encontrado com ID: {Id}", entregadorId);
                throw new KeyNotFoundException($"Entregador com ID {entregadorId} não encontrado.");
            }

            var normalizedDisponibilidade = RemoverAcentos(novaDisponibilidade)
                    .Trim()
                    .ToLower();

            switch (normalizedDisponibilidade) 
            { 
                case "disponivel":
                    buscaEntregador.Disponivel = true;
                    _logger.LogInformation("Entregador {Nome} agora está disponível.", buscaEntregador.Nome);
                    break;
                case "indisponivel":
                    buscaEntregador.Disponivel = false;
                    _logger.LogInformation("Entregador {Nome} agora está indisponível.", buscaEntregador.Nome);
                     break;

                default: _logger.LogWarning("Valor inválido: {Disponibilidade}", novaDisponibilidade);
                    throw new ArgumentException("Disponibilidade deve ser 'disponível' ou 'indisponível'.");


            }

            await _entregadorRepository.UpdateAsync(buscaEntregador);
            await _unitOfWork.CommitAsync();
            return buscaEntregador;

        }

        // Método auxiliar para remover acentos de uma string
        public static string RemoverAcentos(string texto)
        {
            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = Char.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
