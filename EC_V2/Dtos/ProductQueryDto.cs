namespace EC_V2.Dtos
{
    public class ProductQueryDto
    {
        public string? Cursor { get; set; }
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; } = "id";
        public string SortOrder { get; set; } = "asc";
    }
}
