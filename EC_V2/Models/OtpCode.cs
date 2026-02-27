namespace EC_V2.Models
{
    public class OtpCode
    {
        public int Id { get; set; }
        public string CodeHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public string Purpose { get; set; } // "Register", "Login", "ResetPassword"
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
