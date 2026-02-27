namespace EC_V2.Dtos
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public CategoryDto? ParentCategory { get; set; } // nested parent
    }
}
