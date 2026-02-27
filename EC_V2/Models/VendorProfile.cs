using System.ComponentModel.DataAnnotations.Schema;

namespace EC_V2.Models
{
    public class VendorProfile
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string Description { get; set; }
        public string TaxNumber { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }

    }
}
