using EC_V2.Models;

namespace EC_V2.Dtos
{
    public class CartDto
    {
        public List<CartItem> Items { get; set; }
        public decimal GrandTotal => Items.Sum(i => i.Total);
    }
}
