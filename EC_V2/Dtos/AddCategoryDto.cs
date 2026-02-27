namespace EC_V2.Dtos
{
    public class AddCategoryDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; } 
    }
}
