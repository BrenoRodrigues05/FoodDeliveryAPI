using FoodDeliveryAPI.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(50,ErrorMessage = "O nome precisa ter menos de 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email do cliente é obrigatório.")]
        [EmailAddress(ErrorMessage = "O enderço de E-mail não é válido.")]
        public string Email { get; set; }

        public Endereco Endereco { get; set; } = null!;

        [Required(ErrorMessage = "A senha do cliente é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha precisa ter entre 6 e 100 caracteres.")]

        public Usuario Usuario { get; set; }

        public ICollection<Pedido>? Pedidos { get; set; } = new List<Pedido>();
    }
}
