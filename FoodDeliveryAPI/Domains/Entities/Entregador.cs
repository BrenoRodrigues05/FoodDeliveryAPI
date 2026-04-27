using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class Entregador
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O nome do entregador é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O E-mail do entregador é obrigatório.")]
        [EmailAddress(ErrorMessage = "O E-mail do entregador não é válido.")]
        public string Email { get; set; }

        public bool Disponivel { get; set; } = true;

        public Usuario Usuario { get; set; }

        public ICollection<Pedido>? Pedidos { get; set; } = new List<Pedido>();
    }
}
