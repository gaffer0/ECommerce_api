namespace EC_V2.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }

        public string? VendorId { get; set; }
        public AppUser? Vendor { get; set; }
        public ICollection<Category> Categories { get; set; } // many-to-many
    }
}
