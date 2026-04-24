using AutoMapper;
using FoodDeliveryAPI.Application.DTOs;
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
        private readonly IMapper _mapper;
        private readonly IPalavrasProibidasService _palavrasProibidasService;

        public EntregadorService(IEntregadorRepository entregadorRepository, IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork, ILogger<EntregadorService> logger, IMapper mapper, IPalavrasProibidasService palavrasProibidasService)
        {
            _entregadorRepository = entregadorRepository;
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _palavrasProibidasService = palavrasProibidasService;
        }

        public async Task<IEnumerable<EntregadorResponseDTO>> GetEntregadoresAsync()
        {
            _logger.LogInformation("Recuperando lista de entregadores.");
            var entregadores = await _entregadorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EntregadorResponseDTO>>(entregadores);
        }

        public async Task<EntregadorResponseDTO> GetEntregadorByIdAsync(int id)
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
            return _mapper.Map<EntregadorResponseDTO>(busca);
        }
        public async Task<EntregadorResponseDTO> CreateEntregadorAsync(EntregadorCreateDTO entregador)
        {
           if(entregador == null)
            {
                _logger.LogWarning("Dados do entregador são nulos.");
                throw new ArgumentNullException(nameof(entregador), "Dados do entregador não podem ser nulos.");
            }
            var busca = await _entregadorRepository.GetByNameAsync(entregador.Nome);

            if(busca != null)
            {
                _logger.LogWarning("Entregador já existe com nome: {Nome}", entregador.Nome);
                throw new InvalidOperationException($"Entregador com nome {entregador.Nome} já existe.");
            }

            if (await _palavrasProibidasService.ContemPalavraProibida(entregador.Nome))
            {
                _logger.LogWarning("Nome do entregador contém palavras proibidas: {Nome}", entregador.Nome);
                throw new InvalidOperationException("Nome do entregador contém palavras proibidas.");
            }

            var novoEntregador = _mapper.Map<Entregador>(entregador);
            await _entregadorRepository.CreateAsync(novoEntregador);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Entregador criado: {Nome}", novoEntregador.Nome);

            return _mapper.Map<EntregadorResponseDTO>(novoEntregador);

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

            var buscaPedidos = await _pedidoRepository.GetEmTransitoByEntregador(id);

           if (buscaPedidos != null)
            {
                _logger.LogWarning("Não é possível deletar o entregador {Nome} porque ele tem pedidos em trânsito.", busca.Nome);
                throw new InvalidOperationException($"Não é possível deletar o entregador {busca.Nome} porque ele tem pedidos em trânsito.");
            }

            await _entregadorRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Entregador deletado: {Nome}", busca.Nome);
            return true;
        }

        public async Task<EntregadorResponseDTO> AtualizarDisponibilidadeEntregadorAsync(int entregadorId, bool novaDisponibilidade)
        {
            if(entregadorId <= 0)
            {
                _logger.LogWarning("ID do entregador é inválido: {Id}", entregadorId);
                throw new ArgumentException("ID do entregador é inválido.", nameof(entregadorId));
            }
            var busca = await _entregadorRepository.GetByIdAsync(entregadorId);

            if(busca == null)
            {
                _logger.LogWarning("Entregador não encontrado com ID: {Id}", entregadorId);
                throw new KeyNotFoundException($"Entregador com ID {entregadorId} não encontrado.");
            }

            var buscaPedidos = await _pedidoRepository.GetEmTransitoByEntregador(entregadorId);

            if(buscaPedidos != null && novaDisponibilidade == true)
            {
                _logger.LogWarning("Não é possível disponibilizar o entregador {Nome} porque ele tem pedidos em trânsito.", busca.Nome);
                throw new InvalidOperationException($"Não é possível disponibilizar o entregador {busca.Nome} porque ele tem pedidos em trânsito.");
            }

            if (novaDisponibilidade == busca.Disponivel)
            {
                _logger.LogInformation("Entregador {Nome} já está com a disponibilidade {Disponibilidade}.", busca.Nome, novaDisponibilidade);
                return _mapper.Map<EntregadorResponseDTO>(busca);
            }

            busca.Disponivel = novaDisponibilidade;

            await _entregadorRepository.UpdateAsync(busca);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Disponibilidade do entregador {Nome} atualizada para {Disponibilidade}.", busca.Nome, novaDisponibilidade);
            return _mapper.Map<EntregadorResponseDTO>(busca);

        }

    }
}
