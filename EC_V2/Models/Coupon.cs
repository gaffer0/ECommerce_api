using EC_V2.Models.Enums;

namespace EC_V2.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal MinOrderAmount { get; set; } = 0;
        public int? MaxUses { get; set; } = 1;
        public int UsedCount { get; set; } = 0;
        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
