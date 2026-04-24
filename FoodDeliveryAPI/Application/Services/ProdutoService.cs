using AutoMapper;
using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Infrastructure.Repositories;
using FoodDeliveryAPI.Infrastructure.UnitOfWork;

namespace FoodDeliveryAPI.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProdutoService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPalavrasProibidasService _palavrasProibidasService;

        public ProdutoService(IProdutoRepository produtoRepository, IPedidoRepository pedidoRepository, IMapper mapper,
            ILogger<ProdutoService> logger, IUnitOfWork unitOfWork, IPalavrasProibidasService palavrasProibidasService)
        {
            _produtoRepository = produtoRepository;
            _pedidoRepository = pedidoRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _palavrasProibidasService = palavrasProibidasService;
        }

        public async Task<ProdutoResponseDTO> CreateProdutoAsync(ProdutoCreateDTO produto)
        {
            if(string.IsNullOrEmpty(produto.Nome) || produto.Preco <= 0)
            {
                _logger.LogWarning("Tentativa de criar produto com nome vazio ou preço inválido.");
                throw new ArgumentException("O nome do produto não pode ser vazio e o preço deve ser maior que zero.");
            }

            var palavrasProibidas = await _palavrasProibidasService.ContemPalavraProibida(produto.Nome);

            if(palavrasProibidas)
            {
                _logger.LogWarning("Tentativa de criar produto com nome contendo palavras proibidas: {Nome}", produto.Nome);
                throw new ArgumentException("O nome do produto contém palavras proibidas.");
            }

            var buscarProduto = await _produtoRepository.GetProdutosByNomeAsync(produto.Nome);
            
            if(buscarProduto.Any())
            {
                _logger.LogWarning("Tentativa de criar produto com nome já existente: {Nome}", produto.Nome);
                throw new InvalidOperationException("Já existe um produto com esse nome.");
            }

            var produtoEntity = _mapper.Map<Produto>(produto);

            await _produtoRepository.CreateProdutoAsync(produtoEntity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Produto criado com sucesso: {Nome}", produto.Nome);

            return _mapper.Map<ProdutoResponseDTO>(produtoEntity);
        }

        public async Task<ProdutoResponseDTO> GetProdutoByIdAsync(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Tentativa de buscar produto com ID inválido: {Id}", id);
                throw new ArgumentException("O ID do produto deve ser maior que zero.");
            }

            var produtoEntity = await _produtoRepository.GetProdutoByIdAsync(id);

            if(produtoEntity == null)
            {
                _logger.LogWarning("Produto não encontrado com ID: {Id}", id);
                throw new KeyNotFoundException("Produto não encontrado.");
            }

            _logger.LogInformation("Produto encontrado com ID: {Id}", id);
            return _mapper.Map<ProdutoResponseDTO>(produtoEntity);
        }

        public async Task<IEnumerable<ProdutoResponseDTO>> GetProdutosByNomeAsync(string nome)
        {
            if(string.IsNullOrEmpty(nome))
            {
                _logger.LogWarning("Tentativa de buscar produtos com nome vazio.");
                throw new ArgumentException("O nome do produto não pode ser vazio.");
            }

            var produtos = await _produtoRepository.GetProdutosByNomeAsync(nome);
           
            if(!produtos.Any())
            {
                _logger.LogWarning("Nenhum produto encontrado com nome: {Nome}", nome);
                throw new KeyNotFoundException("Nenhum produto encontrado com esse nome.");
            }

            _logger.LogInformation("Produtos encontrados com nome: {Nome}", nome);

            return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);

        }

        public async Task<IEnumerable<ProdutoResponseDTO>> GetProdutosByPreco(decimal preco)
        {
            switch (preco)
            {
                case <= 0:
                    _logger.LogWarning("Tentativa de buscar produtos com preço inválido: {Preco}", preco);
                    throw new ArgumentException("O preço deve ser maior que zero.");

                    case > 1000:
                    _logger.LogWarning("Tentativa de buscar produtos com preço excessivo: {Preco}", preco);
                                        throw new ArgumentException("O preço deve ser razoável e não pode exceder 1.000.");

                    case var _ when decimal.Round(preco, 2) != preco:
                    _logger.LogWarning("Tentativa de buscar produtos com preço com mais de duas casas decimais: {Preco}", preco);
                    throw new ArgumentException("O preço deve ter no máximo duas casas decimais.");
            }

            var buscarProdutos = await _produtoRepository.GetProdutosByPrecoAsync(preco);

            if(!buscarProdutos.Any())
            {
                _logger.LogWarning("Nenhum produto encontrado com preço: {Preco}", preco);
                throw new KeyNotFoundException("Nenhum produto encontrado com esse preço.");
            }

            _logger.LogInformation("Produtos encontrados com preço: {Preco}", preco);

            return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(buscarProdutos);
        }

        public async Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoUpdateDTO produto)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Tentativa de atualizar produto com ID inválido: {Id}", id);
                throw new ArgumentException("O ID do produto deve ser maior que zero.");
            }

            var produtoEntity = await _produtoRepository.GetProdutoByIdAsync(id);

            if(produtoEntity == null)
            {
                _logger.LogWarning("Produto não encontrado para atualização com ID: {Id}", id);
                throw new KeyNotFoundException("Produto não encontrado para atualização.");
            }

            _mapper.Map(produto, produtoEntity);
            await _produtoRepository.UpdateProdutoAsync(produtoEntity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Produto atualizado com sucesso: {Id}", id);

            return _mapper.Map<ProdutoResponseDTO>(produtoEntity);
        }

        public async Task<bool> DeleteProdutoByIdAsync(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Tentativa de deletar produto com ID inválido: {Id}", id);
                throw new ArgumentException("O ID do produto deve ser maior que zero.");
            }

            var produtoEntity = await _produtoRepository.GetProdutoByIdAsync(id);

            if(produtoEntity == null)
            {
                _logger.LogWarning("Produto não encontrado para exclusão com ID: {Id}", id);
                throw new KeyNotFoundException("Produto não encontrado para exclusão.");
            }

            await _produtoRepository.DeleteProdutoAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Produto deletado com sucesso: {Id}", id);
            return true;
        }

        public async Task<IEnumerable<ProdutoResponseDTO>> GetDisponiveisProdutosAsync()
        {
            var produtosDisponiveis = await _produtoRepository.GetDisponiveisAsync();

            if(!produtosDisponiveis.Any())
            {
                _logger.LogWarning("Nenhum produto disponível encontrado.");
                throw new KeyNotFoundException("Nenhum produto disponível encontrado.");
            }

            _logger.LogInformation("Produtos disponíveis encontrados.");
            return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtosDisponiveis);
        }
    }
}
