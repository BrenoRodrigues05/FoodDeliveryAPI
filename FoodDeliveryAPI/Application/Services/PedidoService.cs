using AutoMapper;
using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Application.Mappings;
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
        private readonly IClienteRepository _clienteRepository;
        private readonly IProdutoRepository _produtoRepository;
        public readonly IMapper _mapper;

        public PedidoService(IPedidoRepository pedidoRepository, IUnitOfWork unitOfWork, ILogger<PedidoService> logger, 
            IEntregadorRepository entregadorRepository, IMapper mapper, IClienteRepository clienteRepository, 
            IProdutoRepository produtoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _entregadorRepository = entregadorRepository;
            _mapper = mapper;
            _clienteRepository = clienteRepository;
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<PedidoResponseDTO>> GetPedidosAsync()
        {
            _logger.LogInformation("Buscando todos os pedidos.");

            var pedidos = await _pedidoRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<PedidoResponseDTO>>(pedidos);

        }

        public async Task<PedidoResponseDTO> GetPedidoByIdAsync(int id)
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
           return _mapper.Map<PedidoResponseDTO>(pedido);
        }

        public async Task<PedidoResponseDTO> CreatePedidoAsync(PedidoCreateDTO dto, int clienteId)
        {
            if (dto == null || dto.Itens == null || !dto.Itens.Any())
            {
                _logger.LogWarning("Pedido inválido.");
                throw new ArgumentException("Pedido deve conter itens.");
            }

            if (clienteId <= 0)
            {
                _logger.LogWarning("ClienteId inválido: {ClienteId}", clienteId);
                throw new ArgumentException("ClienteId inválido.");
            }

            var cliente = await _clienteRepository.GetByIdAsync(clienteId);

            if (cliente == null)
            {
                _logger.LogWarning("Cliente não encontrado: {ClienteId}", clienteId);
                throw new KeyNotFoundException("Cliente não encontrado.");
            }

            var pedidosCliente = await _pedidoRepository.GetByClienteIdAsync(clienteId);

            if (pedidosCliente.Any(p => p.Status == StatusPedido.Pendente || p.Status == StatusPedido.EmTransito))
            {
                _logger.LogWarning("Cliente já possui pedido ativo: {ClienteId}", clienteId);
                throw new InvalidOperationException("Cliente já possui pedido ativo.");
            }

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                Status = StatusPedido.Pendente,
                PedidoItens = new List<PedidoItem>()
            };

            decimal total = 0;
            
            var produtosIds = dto.Itens.Select(i => i.ProdutoId).ToList();
            var produtos = await _produtoRepository.GetByIdsAsync(produtosIds);

            foreach (var itemDto in dto.Itens)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == itemDto.ProdutoId);

                if (produto == null)
                    throw new Exception($"Produto {itemDto.ProdutoId} não encontrado.");

                if (!produto.Disponivel)
                    throw new Exception($"Produto {produto.Nome} indisponível.");

                pedido.PedidoItens.Add(new PedidoItem
                {
                    ProdutoId = produto.Id,
                    Quantidade = itemDto.Quantidade
                });
            }

            pedido.ValorTotal = total;

            await _pedidoRepository.AddAsync(pedido);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Pedido criado com sucesso para cliente {ClienteId}", clienteId);

            return _mapper.Map<PedidoResponseDTO>(pedido);

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

        public async Task<PedidoResponseDTO> AtribuirEntregadorAsync(int pedidoId, int entregadorId)
        {
            if(pedidoId <= 0)
            {
                _logger.LogWarning("ID de pedido inválido para atribuição de entregador: {PedidoId}", pedidoId);
                throw new ArgumentException("ID de pedido deve ser maior que zero.", nameof(pedidoId));
            }
            if(entregadorId <= 0)
            {
                _logger.LogWarning("ID de entregador inválido para atribuição ao pedido com ID: {PedidoId}. ID de entregador: {EntregadorId}", pedidoId, entregadorId);
                throw new ArgumentException("ID de entregador deve ser maior que zero.", nameof(entregadorId));
            }   
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

            var entregador = await _entregadorRepository.GetPedidosEntregador(entregadorId);
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

            return _mapper.Map<PedidoResponseDTO>(pedido);

        }

        public async Task<PedidoResponseDTO> AtualizarStatusPedidoAsync(int pedidoId, string novoStatus)
        {
            if(pedidoId < 0)
            {
                _logger.LogWarning("ID de pedido inválido para atualização de status: {PedidoId}", pedidoId);
                throw new ArgumentException("ID de pedido deve ser maior ou igual a zero.", nameof(pedidoId));
            }
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
            
            return _mapper.Map<PedidoResponseDTO>(pedido);
        }
    }

 }
