using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryAPI.Domains.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
