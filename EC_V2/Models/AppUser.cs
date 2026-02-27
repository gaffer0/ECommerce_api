using Microsoft.AspNetCore.Identity;

namespace EC_V2.Models
{
    public class AppUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public VendorProfile? VendorProfile { get; set; }
        public CustomerProfile? CustomerProfile { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<OtpCode> OtpCodes { get; set; }
    }
}
