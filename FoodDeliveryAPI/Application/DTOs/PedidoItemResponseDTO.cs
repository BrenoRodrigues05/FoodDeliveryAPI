namespace FoodDeliveryAPI.Application.DTOs
{
    public class PedidoItemResponseDTO
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }

        public ProdutoResponseDTO? Produto { get; set; }
    }
}
