using EC_V2.Models.Enums;
using static Azure.Core.HttpHeader;

namespace EC_V2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string ShippingAddress { get; set; } = null!;

        // Financial breakdown
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }

        // Optional coupon
        public int? CouponId { get; set; }

        // Navigation properties
        public AppUser Customer { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Coupon? Coupon { get; set; }



    }
}
