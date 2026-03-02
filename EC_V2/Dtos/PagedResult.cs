namespace EC_V2.Dtos
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
    }
}
