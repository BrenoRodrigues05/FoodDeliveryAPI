using FoodDeliveryAPI.Domains.Entities.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O status do pedido é obrigatório.")]
        public StatusPedido Status { get; set; }

        public int ClienteId { get; set; }

        public int? EntregadorId { get; set; }

        public decimal ValorTotal { get; set; }

        public ICollection<PedidoItem> PedidoItens { get; set; } = new Collection<PedidoItem>();

        public Entregador? Entregador { get; set; } 

        public Cliente Cliente { get; set; } = null!;

    }
}
