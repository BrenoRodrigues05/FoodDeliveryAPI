using AutoMapper;
using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Infrastructure.Repositories;
using FoodDeliveryAPI.Infrastructure.UnitOfWork;

namespace FoodDeliveryAPI.Application.Services
{
    public class EnderecoService : IEnderecoService
    {
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EnderecoService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public EnderecoService(IEnderecoRepository enderecoRepository, IClienteRepository clienteRepository, 
            IMapper mapper, ILogger<EnderecoService> logger, IUnitOfWork unitOfWork)
        {
            _enderecoRepository = enderecoRepository;
            _clienteRepository = clienteRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<EnderecoResponseDTO> ObterEnderecoPorIdAsync(int id)
        {
          if(id <= 0)
            {
                _logger.LogWarning("Tentativa de obter endereço com ID inválido: {Id}", id);
                throw new ArgumentException("ID do endereço deve ser maior que zero.");
            }

          var endereco = await _enderecoRepository.GetEnderecoByIdAsync(id);
          if (endereco == null)
          {
              _logger.LogWarning("Endereço com ID {Id} não encontrado.", id);
                throw new KeyNotFoundException($"Endereço com ID {id} não encontrado.");
            }
          _logger.LogInformation("Endereço com ID {Id} obtido com sucesso.", id);
            return _mapper.Map<EnderecoResponseDTO>(endereco);
        }

        public async Task<EnderecoResponseDTO> CriarEnderecoAsync(EnderecoCreateDTO endereco, int clienteId)
        {
           if(string.IsNullOrWhiteSpace(endereco.Nome) || string.IsNullOrWhiteSpace(endereco.Rua) || string.IsNullOrWhiteSpace(endereco.Numero))
            {
                _logger.LogWarning("Tentativa de criar endereço com dados incompletos.");
                throw new ArgumentException("Nome, Rua e Número são campos obrigatórios.");
            }
            
           var buscaCliente = await _clienteRepository.GetByIdAsync(clienteId);
            if(buscaCliente == null)
            {
                _logger.LogWarning("Tentativa de criar endereço para cliente inexistente com ID: {ClienteId}", clienteId);
                throw new KeyNotFoundException($"Cliente com ID {clienteId} não encontrado.");
            }

            if(buscaCliente.Endereco != null)
            {
                _logger.LogWarning("Tentativa de criar endereço para cliente com ID {ClienteId} que já possui um endereço.", clienteId);
                throw new InvalidOperationException($"Cliente com ID {clienteId} já possui um endereço cadastrado.");
            }

            var enderecoEntity = _mapper.Map<Endereco>(endereco);
            enderecoEntity.ClienteId = clienteId;

            await _enderecoRepository.CreateEnderecoAsync(enderecoEntity);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Endereço criado com sucesso para o cliente com ID {ClienteId}.", clienteId);
            return _mapper.Map<EnderecoResponseDTO>(enderecoEntity);
        }

        public async Task<EnderecoResponseDTO> AtualizarEnderecoAsync(int enderecoId, EnderecoUpdateDTO endereco, int clienteId)
        {
            if (string.IsNullOrWhiteSpace(endereco.Nome) || string.IsNullOrWhiteSpace(endereco.Rua) || string.IsNullOrWhiteSpace(endereco.Numero))
            {
                _logger.LogWarning("Tentativa de atualizar endereço com dados incompletos para ID: {Id}", enderecoId);
                throw new ArgumentException("Nome, Rua e Número são campos obrigatórios.");
            }
            if(enderecoId <= 0 || clienteId <= 0)
            {
                _logger.LogWarning("Tentativa de atualizar endereço com ID ou cliente ID inválidos: {EnderecoId}, {ClienteId}", enderecoId, clienteId);
                throw new ArgumentException("ID do endereço e ID do cliente devem ser maiores que zero.");
            }
            var buscaCliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (buscaCliente == null)
            {
                _logger.LogWarning("Tentativa de atualizar endereço para cliente inexistente com ID: {ClienteId}", clienteId);
                throw new KeyNotFoundException($"Cliente com ID {clienteId} não encontrado.");
            }

            var enderecoDoCliente = await _enderecoRepository.GetEnderecoByClienteIdAsync(clienteId);

            if(enderecoDoCliente == null || enderecoDoCliente.Id != enderecoId)
            {
                _logger.LogWarning("Tentativa de atualizar endereço com ID {EnderecoId} que não pertence ao cliente com ID {ClienteId}.", enderecoId, clienteId);
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não pertence ao cliente com ID {clienteId}.");
            }

            if (enderecoDoCliente.Id != enderecoId)
            {
                _logger.LogWarning("Tentativa de atualizar endereço com ID {EnderecoId} que não pertence ao cliente com ID {ClienteId}.", enderecoId, clienteId);
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não pertence ao cliente com ID {clienteId}.");
            }

            enderecoDoCliente.Nome = endereco.Nome;
            enderecoDoCliente.Rua = endereco.Rua;
            enderecoDoCliente.Numero = endereco.Numero;
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Endereço com ID {Id} atualizado com sucesso para o cliente com ID {ClienteId}.", enderecoId, clienteId);
            return _mapper.Map<EnderecoResponseDTO>(enderecoDoCliente);
        }

        public async Task<bool> ExcluirEnderecoAsync(int enderecoId, int clienteId)
        {
            if(enderecoId <= 0 || clienteId <= 0)
            {
                _logger.LogWarning("Tentativa de excluir endereço com ID ou cliente ID inválidos: {EnderecoId}, {ClienteId}", enderecoId, clienteId);
                throw new ArgumentException("ID do endereço e ID do cliente devem ser maiores que zero.");
            }
           
            var buscaCliente = await _clienteRepository.GetByIdAsync(clienteId);
            if(buscaCliente == null)
            {
                _logger.LogWarning("Tentativa de excluir endereço para cliente inexistente com ID: {ClienteId}", clienteId);
                throw new KeyNotFoundException($"Cliente com ID {clienteId} não encontrado.");
            }
            var buscaEndereco = await _enderecoRepository.GetEnderecoByIdAsync(enderecoId);
            if(buscaEndereco == null)
            {
                _logger.LogWarning("Endereço com ID {Id} não encontrado.", enderecoId);
                throw new KeyNotFoundException($"Endereço com ID {enderecoId} não encontrado.");
            }
            var enderecoDoCliente = await _enderecoRepository.GetEnderecoByClienteIdAsync(clienteId);
            if (enderecoDoCliente == null)
            {
                _logger.LogWarning("Tentativa de excluir endereço inexistente para cliente com ID: {ClienteId}", clienteId);
                throw new KeyNotFoundException($"Endereço para cliente com ID {clienteId} não encontrado.");
            } 
            if(enderecoDoCliente.Id != enderecoId)
            {
                _logger.LogWarning("Tentativa de excluir endereço com ID {EnderecoId} que não pertence ao cliente com ID {ClienteId}.", enderecoId, clienteId);
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não pertence ao cliente com ID {clienteId}.");
            }
            await _enderecoRepository.DeleteEnderecoAsync(enderecoId);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Endereço com ID {Id} excluído com sucesso para o cliente com ID {ClienteId}.", enderecoId, clienteId);
            return true;
        }

        public async Task<EnderecoResponseDTO> ObterEnderecoDoCliente(int clienteId)
        {
            if(clienteId <= 0)
            {
                _logger.LogWarning("Tentativa de obter endereço para cliente com ID inválido: {ClienteId}", clienteId);
                throw new ArgumentException("ID do cliente deve ser maior que zero.");
            }
            var buscaCliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (buscaCliente == null)
            {
                _logger.LogWarning("Tentativa de obter endereço para cliente inexistente com ID: {ClienteId}", clienteId);
                throw new KeyNotFoundException($"Cliente com ID {clienteId} não encontrado.");
            }
            var endereco = await _enderecoRepository.GetEnderecoByClienteIdAsync(clienteId);
            if (endereco == null)
            {
                _logger.LogWarning("Endereço para cliente com ID {ClienteId} não encontrado.", clienteId);
                throw new KeyNotFoundException($"Endereço para cliente com ID {clienteId} não encontrado.");
            }
            _logger.LogInformation("Endereço para cliente com ID {ClienteId} obtido com sucesso.", clienteId);
            return _mapper.Map<EnderecoResponseDTO>(endereco);
        }
    }
}
