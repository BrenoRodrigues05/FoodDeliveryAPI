namespace FoodDeliveryAPI.Application.Auth.PasswordHash
{
    public interface IPasswordService
    {
        public string HashPassword(string senha);
        bool Verification(string senhaDigitada, string senhaHash);
    }
}
