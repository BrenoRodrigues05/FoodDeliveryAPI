namespace FoodDeliveryAPI.Domains.Entities
{
    public class PalavraProibida
    {
        public int Id { get; set; }
        public string Palavra { get; private set; }
        
        public PalavraProibida(string palavra)
        {
            Palavra = palavra.ToLower();
           
        }


    }
}
