using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class Entregador
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do entregador é obrigatório.")]
        public string Nome { get; set; }

        public bool Disponivel { get; set; } = false;

        public ICollection<Pedido>? Pedidos { get; set; } = new List<Pedido>();
    }
}
