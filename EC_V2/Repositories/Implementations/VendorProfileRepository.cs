using EC_V2.Data;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Repositories.Implementations
{
    public class VendorProfileRepository : GenericRepository<VendorProfile>, IVendorProfileRepository
    {
        public VendorProfileRepository(AppDbContext context) : base(context)
        { }

        public async Task<VendorProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<VendorProfile>().FirstOrDefaultAsync(vp => vp.UserId == userId);
        }
    }
}
