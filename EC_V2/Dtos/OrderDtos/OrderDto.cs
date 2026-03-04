using EC_V2.Models;

namespace EC_V2.Dtos.OrderDtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
