using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(50,ErrorMessage = "O nome precisa ter menos de 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email do cliente é obrigatório.")]
        [EmailAddress(ErrorMessage = "O enderço de E-mail não é válido.")]
        public string Email { get; set; }

        public Endereco Endereco { get; set; } = null!;

        public ICollection<Pedido>? Pedidos { get; set; } = new List<Pedido>();
    }
}
