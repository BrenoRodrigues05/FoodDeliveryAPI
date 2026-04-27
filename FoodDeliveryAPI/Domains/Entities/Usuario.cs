namespace FoodDeliveryAPI.Domains.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; } 

        public string Senha { get; set; }

        public string Tipo { get; set; } // Exemplo: "Cliente", "Entregador", "Administrador"
    }
}
