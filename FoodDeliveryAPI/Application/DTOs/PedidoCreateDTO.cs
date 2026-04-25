namespace FoodDeliveryAPI.Application.DTOs
{
    public class PedidoCreateDTO
    {
        public int ClienteId { get; set; }

        public decimal ValorTotal { get; set; }
        public List<PedidoItemCreateDTO> Itens { get; set; } = new();
    }
}
