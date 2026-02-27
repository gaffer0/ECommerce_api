namespace EC_V2.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
