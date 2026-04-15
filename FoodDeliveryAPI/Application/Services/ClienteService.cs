using AutoMapper;
using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Domains.Entities.Enums;
using FoodDeliveryAPI.Infrastructure.Repositories;
using FoodDeliveryAPI.Infrastructure.UnitOfWork;

namespace FoodDeliveryAPI.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ILogger<ClienteService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository clienteRepository, IEnderecoRepository enderecoRepository, IPedidoRepository
            pedidoRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ClienteService> logger)
        {
            _clienteRepository = clienteRepository;
            _enderecoRepository = enderecoRepository;
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ClienteResponseDTO>> GetAllClientesAsync()
        {
            _logger.LogInformation("Buscando todos os clientes");
            var busca = await _clienteRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClienteResponseDTO>>(busca);
        }

        public async Task<ClienteResponseDTO> GetClienteByIdAsync(int id)
        {
            if (id < 0)
            {
                _logger.LogWarning("ID do cliente inválido: {Id}", id);
                throw new ArgumentException("ID do cliente deve ser um número positivo.", nameof(id));
            }

            var busca = await _clienteRepository.GetByIdAsync(id);
            _logger.LogInformation("Buscando cliente com ID: {Id}", id);
            if (busca == null)
            {
                _logger.LogWarning("Cliente não encontrado com ID: {Id}", id);
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
            }
            _logger.LogInformation("Cliente encontrado com ID: {Id}", id);
            return _mapper.Map<ClienteResponseDTO>(busca);
        }

        public async Task<ClienteResponseDTO> GetClienteByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email do cliente é nulo ou vazio.");
                throw new ArgumentException("Email do cliente não pode ser nulo ou vazio.", nameof(email));
            }
            var busca = await _clienteRepository.GetByEmailAsync(email);

            if (busca == null)
            {
                _logger.LogWarning("Cliente não encontrado com email: {Email}", email);
                throw new KeyNotFoundException($"Cliente com email {email} não encontrado.");
            }
            _logger.LogInformation("Cliente encontrado com email: {Email}", email);
            return _mapper.Map<ClienteResponseDTO>(busca);
        }

        public async Task<ClienteResponseDTO> CreateClienteAsync(ClienteCreateDTO cliente)
        {
            if (string.IsNullOrEmpty(cliente.Nome) || string.IsNullOrEmpty(cliente.Email))
            {
                _logger.LogWarning("Nome ou email do cliente é nulo ou vazio.");
                throw new ArgumentException("Nome e email do cliente não podem ser nulos ou vazios.");
            }

            var busca = await _clienteRepository.GetByEmailAsync(cliente.Email);

            if (busca != null)
            {
                _logger.LogWarning("Cliente já existe com email: {Email}", cliente.Email);
                throw new InvalidOperationException($"Cliente com email {cliente.Email} já existe.");
            }

            var novoCliente = _mapper.Map<Cliente>(cliente);

            await _clienteRepository.CreateAsync(novoCliente);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Cliente criado com sucesso: {Email}", cliente.Email);
            return _mapper.Map<ClienteResponseDTO>(novoCliente);
        }
        public async Task<ClienteResponseDTO> UpdateClienteAsync(int id, ClienteUpdateDTO cliente)
        {
            if (id < 0)
            {
                _logger.LogWarning("ID do cliente inválido: {Id}", id);
                throw new ArgumentException("ID do cliente deve ser um número positivo.", nameof(id));
            }
            if (string.IsNullOrEmpty(cliente.Nome) || string.IsNullOrEmpty(cliente.Email))
            {
                _logger.LogWarning("Nome e email do cliente são nulos ou vazios.");
                throw new ArgumentException("Nome e email do cliente não podem ser nulos ou vazios.");
            }

            var busca = await _clienteRepository.GetByIdAsync(id);

            if (busca == null)
            {
                _logger.LogWarning("Cliente não encontrado com ID: {Id}", id);
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
            }

            busca.Nome = cliente.Nome;
            busca.Email = cliente.Email;

            await _clienteRepository.UpdateAsync(busca);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Cliente atualizado com sucesso: {Id}", id);

            return _mapper.Map<ClienteResponseDTO>(busca);
        }
        public async Task<bool> DeleteClienteAsync(int id)
        {
            if (id < 0)
            {
                _logger.LogWarning("ID do cliente inválido: {Id}", id);
                throw new ArgumentException("ID do cliente deve ser um número positivo.", nameof(id));
            }

            var buscaCliente = await _clienteRepository.GetByIdAsync(id);

            if (buscaCliente == null)
            {
                _logger.LogWarning("Cliente não encontrado com ID: {Id}", id);
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
            }

            var pedidoAndamento = await _pedidoRepository.GetByClienteIdAsync(id);

            if (pedidoAndamento != null && pedidoAndamento.Any(p => p.Status == StatusPedido.EmTransito || p.Status == StatusPedido.Pendente))
            {
                _logger.LogWarning("Cliente com ID {Id} possui um pedido em andamento.", id);
                throw new InvalidOperationException("Não é possível excluir o cliente enquanto houver um pedido em andamento.");
            }

            await _clienteRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation($"Cliente com id: {id} deletado com sucesso.");

            return true;

        }

        public async Task<EnderecoResponseDTO> BuscarEnderecoClienteAsync(int clienteId)
        {
            if (clienteId < 0)
            {
                _logger.LogWarning("ID do cliente inválido: {Id}", clienteId);
                throw new ArgumentException("ID do cliente deve ser um número positivo.", nameof(clienteId));
            }

            _logger.LogInformation("Buscando endereço do cliente com ID: {Id}", clienteId);

            var busca = await _enderecoRepository.GetEnderecoByClienteIdAsync(clienteId);

            if (busca == null)
            {
                _logger.LogWarning("Endereço não encontrado para cliente com ID: {Id}", clienteId);
                throw new KeyNotFoundException($"Endereço para cliente com ID {clienteId} não encontrado.");
            }

            _logger.LogInformation("Endereço encontrado para cliente com ID: {Id}", clienteId);

            return _mapper.Map<EnderecoResponseDTO>(busca);
        }

        public async Task<PedidoResponseDTO> BuscarPedidosClienteAsync(int clienteId)
        {
            if (clienteId < 0)
            {
                _logger.LogWarning("ID do cliente inválido: {Id}", clienteId);
                throw new ArgumentException("ID do cliente deve ser um número positivo.", nameof(clienteId));
            }

            var busca = await _pedidoRepository.GetByClienteIdAsync(clienteId);

            if (busca == null)
            {
                _logger.LogWarning("Pedidos não encontrados para cliente com ID: {Id}", clienteId);
                throw new KeyNotFoundException($"Pedidos para cliente com ID {clienteId} não encontrado.");
            }

            _logger.LogInformation("Pedidos encontrados para cliente com ID: {Id}", clienteId);

            return _mapper.Map<PedidoResponseDTO>(busca);
        }

    }
}
