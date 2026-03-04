namespace EC_V2.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        public string ShippingAddress { get; set; }
        
        public string? CouponCode { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
