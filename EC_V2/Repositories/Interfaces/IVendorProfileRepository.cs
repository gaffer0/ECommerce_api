using EC_V2.Models;

namespace EC_V2.Repositories.Interfaces
{
    public interface IVendorProfileRepository : IGenericRepository<VendorProfile>
    {
        Task<VendorProfile?> GetByUserIdAsync(string userId);
    }
}
