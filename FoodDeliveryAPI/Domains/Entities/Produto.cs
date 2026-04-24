using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(50, ErrorMessage = "Nome muito longo. Precisa conter menos de 50 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O valor do produto é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
 
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
        [StringLength(200, ErrorMessage = "Descrição muito longa. Precisa conter menos de 200 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A disponibilidade do produto é obrigatória.")]
        public bool Disponivel { get; set; }

        public ICollection<PedidoItem>? PedidoItens { get; set; } = new List<PedidoItem>();

    }
}
