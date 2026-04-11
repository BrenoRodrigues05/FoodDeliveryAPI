namespace FoodDeliveryAPI.Application.DTOs
{
    public class ClienteResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public EnderecoResponseDTO? Endereco { get; set; }
    }
}
