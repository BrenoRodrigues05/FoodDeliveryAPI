using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Domains.Entities.Enums;
using FoodDeliveryAPI.Infrastructure.Repositories;
using FoodDeliveryAPI.Infrastructure.UnitOfWork;

namespace FoodDeliveryAPI.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PedidoService> _logger;
        private readonly IEntregadorRepository _entregadorRepository;

        public PedidoService(IPedidoRepository pedidoRepository, IUnitOfWork unitOfWork, ILogger<PedidoService> logger, IEntregadorRepository entregadorRepository)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _entregadorRepository = entregadorRepository;
        }

        public async Task<IEnumerable<Pedido>> GetPedidosAsync()
        {
            _logger.LogInformation("Buscando todos os pedidos.");

            var pedidos = await _pedidoRepository.GetAllAsync();

            return pedidos;

        }

        public async Task<Pedido> GetPedidoByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de pedido inválido: {Id}", id);
                throw new ArgumentException("ID de pedido deve ser maior que zero.", nameof(id));
            }
            _logger.LogInformation("Buscando pedido com ID: {Id}", id);
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
            {
                _logger.LogWarning("Pedido não encontrado com ID: {Id}", id);
                throw new KeyNotFoundException($"Pedido com ID {id} não encontrado.");
            }
            return pedido;
        }

        public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
        {
            if (pedido == null)
            {
                _logger.LogWarning("Tentativa de criar pedido com dados nulos.");
                throw new ArgumentNullException(nameof(pedido), "Pedido não pode ser nulo.");
            }

            var novoPedido = await _pedidoRepository.AddAsync(pedido);

            novoPedido.Status = StatusPedido.Pendente;

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Pedido criado com ID: {Id}", novoPedido.Id);

            return novoPedido;
        }

        public async Task<bool> DeletePedidoAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de pedido inválido para exclusão: {Id}", id);
                throw new ArgumentException("ID de pedido deve ser maior que zero.", nameof(id));
            }

            var verificaPedido = await _pedidoRepository.GetByIdAsync(id);

            if (verificaPedido == null)
            {
                _logger.LogWarning("Pedido não encontrado para exclusão com ID: {Id}", id);
                throw new KeyNotFoundException($"Pedido com ID {id} não encontrado para exclusão.");
            }

            if (verificaPedido.Status != StatusPedido.Pendente)
            {
                _logger.LogWarning("Não é permitido excluir o pedido com ID: {Id} porque o status do pedido é '{StatusAtual}'.", id, verificaPedido.Status);
                throw new InvalidOperationException($"Não é permitido excluir o pedido com ID {id} porque o status do pedido é '{verificaPedido.Status}'.");
            }

            await _pedidoRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Pedido com ID: {Id} excluído com sucesso.", id);

            return true;

        }

        public async Task<Pedido> AtribuirEntregador(int pedidoId, int entregadorId)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
            {
                _logger.LogWarning("Pedido não encontrado para atribuição de entregador com ID: {PedidoId}", pedidoId);
                throw new KeyNotFoundException($"Pedido com ID {pedidoId} não encontrado para atribuição de entregador.");
            }
            if (pedido.Status != StatusPedido.Pendente)
            {
                _logger.LogWarning("Não é permitido atribuir entregador ao pedido com ID: {PedidoId} porque o status do pedido é '{StatusAtual}'.", pedidoId, pedido.Status);
                throw new InvalidOperationException($"Não é permitido atribuir entregador ao pedido com ID {pedidoId} porque o status do pedido é '{pedido.Status}'.");
            }

            var entregador = await _entregadorRepository.GetByIdAsync(entregadorId);
            if (entregador == null)
            {
                _logger.LogWarning("Entregador não encontrado para atribuição com ID: {EntregadorId}", entregadorId);
                throw new KeyNotFoundException($"Entregador com ID {entregadorId} não encontrado para atribuição.");
            }
            if (entregador.Disponivel == false)
            {
                _logger.LogWarning("Não é permitido atribuir entregador com ID: {EntregadorId} ao pedido com ID: {PedidoId} porque o status do entregador é '{StatusAtual}'.", entregadorId, pedidoId, entregador.Disponivel);
                throw new InvalidOperationException($"Não é permitido atribuir entregador com ID {entregadorId} ao pedido com ID {pedidoId} porque o status do entregador é '{entregador.Disponivel}'.");
            }
            if (entregador.Pedidos != null && entregador.Pedidos.Any(p => p.Status == StatusPedido.EmTransito))
            {
                _logger.LogWarning("Não é permitido atribuir entregador com ID: {EntregadorId} ao pedido com ID: {PedidoId} porque o entregador tem pedidos em trânsito.", entregadorId, pedidoId);
                throw new InvalidOperationException($"Não é permitido atribuir entregador com ID {entregadorId} ao pedido com ID {pedidoId} porque o entregador tem pedidos em trânsito.");
            }

            pedido.EntregadorId = entregadorId;
            pedido.Status = StatusPedido.EmTransito;
            entregador.Disponivel = false;

            await _unitOfWork.CommitAsync();

            return await _pedidoRepository.GetByIdAsync(pedidoId);

        }

        public async Task<Pedido> AtualizarStatusPedido(int pedidoId, string novoStatus)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
            {
                _logger.LogWarning("Pedido não encontrado para atualização de status com ID: {PedidoId}", pedidoId);
                throw new KeyNotFoundException($"Pedido com ID {pedidoId} não encontrado para atualização de status.");
            }
            if (string.IsNullOrWhiteSpace(novoStatus))
            {
                _logger.LogWarning("Novo Status para pedido é obrigatório.");
                throw new ArgumentException("Novo status é obrigatório.", nameof(novoStatus));
            }
            var statusValido = Enum.TryParse<StatusPedido>(novoStatus, true, out var status);

            if(statusValido == false)
            {
                _logger.LogWarning("Novo Status para pedido é inválido: {NovoStatus}", novoStatus);
                throw new ArgumentException($"Status '{novoStatus}' é inválido.");
            }

           if(status == pedido.Status)
            {
                _logger.LogWarning("O status do pedido com ID: {PedidoId} já é '{StatusAtual}'.", pedidoId, pedido.Status);
                throw new InvalidOperationException($"O status do pedido com ID {pedidoId} já é '{pedido.Status}'.");
            }
           if(pedido.Status == StatusPedido.EmTransito && status == StatusPedido.Pendente)
            {
                _logger.LogWarning("Não é permitido atualizar o status do pedido com ID: {PedidoId} para '{NovoStatus}' porque o status atual é '{StatusAtual}'.", pedidoId, status, pedido.Status);
                throw new InvalidOperationException($"Não é permitido atualizar o status do pedido com ID {pedidoId} para '{status}' porque o status atual é '{pedido.Status}'.");
            }
            if (pedido.Status == StatusPedido.Entregue || pedido.Status == StatusPedido.Cancelado)
            {
                _logger.LogWarning("Não é permitido atualizar o status do pedido com ID: {PedidoId} para '{NovoStatus}' porque o status atual é '{StatusAtual}'.", pedidoId, status, pedido.Status);
                throw new InvalidOperationException($"Não é permitido atualizar o status do pedido com ID {pedidoId} para '{status}' porque o status atual é '{pedido.Status}'.");
            }
           if(pedido.Status == StatusPedido.Pendente && (status == StatusPedido.Entregue || status == StatusPedido.Cancelado))
            {
                _logger.LogWarning("Não é permitido atualizar o status do pedido com ID: {PedidoId} para '{NovoStatus}' porque o status atual é '{StatusAtual}'.", pedidoId, status, pedido.Status);
                throw new InvalidOperationException($"Não é permitido atualizar o status do pedido com ID {pedidoId} para '{status}' porque o status atual é '{pedido.Status}'.");
            }
            
           pedido.Status = status;

            if (pedido.EntregadorId.HasValue && (status == StatusPedido.Entregue || status == StatusPedido.Cancelado))
            {
                var entregador = await _entregadorRepository.GetByIdAsync(pedido.EntregadorId.Value);
                if (entregador != null)
                {
                    entregador.Disponivel = true;
                }
            }

            await _unitOfWork.CommitAsync();
            _logger.LogInformation(
                "Status do pedido {PedidoId} atualizado para {NovoStatus}",
                pedidoId, status);
            return pedido;
        }
    }

 }
