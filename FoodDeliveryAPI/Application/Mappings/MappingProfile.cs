using AutoMapper;
using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;

namespace FoodDeliveryAPI.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping configurations for Cliente
            CreateMap<Cliente, ClienteCreateDTO>().ReverseMap();
            CreateMap<Cliente, ClienteResponseDTO>();

            // Mapping configurations for Pedido
            CreateMap<Pedido, PedidoCreateDTO>().ReverseMap();
            CreateMap<Pedido, PedidoResponseDTO>().ReverseMap();

            // Mapping configurations for PedidoItem
            CreateMap<PedidoItem, PedidoItemResponseDTO>();
            CreateMap<PedidoItemCreateDTO, PedidoItem>();

            // Mapping configurations for Produto
            CreateMap<Produto, ProdutoCreateDTO>().ReverseMap();
            CreateMap<Produto, ProdutoResponseDTO>().ReverseMap();
            CreateMap<Produto, ProdutoUpdateDTO>();

            // Mapping configurations for Entregador
            CreateMap<Entregador, EntregadorCreateDTO>().ReverseMap();
            CreateMap<Entregador, EntregadorResponseDTO>();

            // Mapping configurations for Endereço
            CreateMap<Endereco, EnderecoCreateDTO>().ReverseMap();
            CreateMap<Endereco, EnderecoResponseDTO>();

            // Mapping configurations for Produto
            CreateMap<Produto, ProdutoCreateDTO>().ReverseMap();
            CreateMap<Produto, ProdutoResponseDTO>();
            CreateMap<Produto, ProdutoUpdateDTO>().ReverseMap();

        }
    }
}
