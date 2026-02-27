namespace EC_V2.Models
{
    public class CustomerProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
