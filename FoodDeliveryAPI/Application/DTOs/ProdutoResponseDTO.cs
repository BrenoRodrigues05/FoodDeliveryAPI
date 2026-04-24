namespace FoodDeliveryAPI.Application.DTOs
{
    public class ProdutoResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }

        public string Descricao { get; set; }

        public bool Disponivel { get; set; }
    }
}
