namespace FoodDeliveryAPI.Application.DTOs
{
    public class PedidoResponseDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int? EntregadorId { get; set; }

        public decimal ValorTotal { get; set; }
        public string Status { get; set; }

        public List<PedidoItemResponseDTO> Itens { get; set; }
    }
}
