namespace EC_V2.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Product> Products { get; set; } // many-to-many
    }
}
