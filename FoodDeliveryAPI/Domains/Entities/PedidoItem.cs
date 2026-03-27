using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class PedidoItem
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }

        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "A quantidade do item é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }

        public Pedido Pedido { get; set; } = null!;

        public Produto Produto { get; set; } = null!;

    }
}
