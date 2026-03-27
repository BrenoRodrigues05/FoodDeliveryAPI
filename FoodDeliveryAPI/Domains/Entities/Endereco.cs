using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class Endereco
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        [Required(ErrorMessage = "O nome do endereço é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do endereço deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A rua é obrigatória.")]
        [StringLength(200, ErrorMessage = "A rua deve ter no máximo 200 caracteres.")]
        public string Rua { get; set; }

        [Required(ErrorMessage = "O número é obrigatório.")]
        [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres.")]
        public string Numero { get; set; }

        public Cliente Cliente { get; set; } = null!;
    }
}
