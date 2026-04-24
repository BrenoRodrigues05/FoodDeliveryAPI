namespace FoodDeliveryAPI.Application.DTOs
{
    public class ProdutoCreateDTO
    {
        public string Nome { get; set; }
        public decimal Preco { get; set; }

        public string Descricao { get; set; }

        public bool Disponivel { get; set; }
    }
}
