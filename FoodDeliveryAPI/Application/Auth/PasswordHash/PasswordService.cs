namespace FoodDeliveryAPI.Application.Auth.PasswordHash
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public bool Verification(string senhaDigitada, string senhaHash)
        {
            return BCrypt.Net.BCrypt.Verify(senhaDigitada, senhaHash);
        }
    }
}
