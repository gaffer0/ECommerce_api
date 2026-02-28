using EC_V2.Data;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Repositories.Implementations
{
    public class CustomerProfileRepository : GenericRepository<CustomerProfile>, ICustomerProfileRepository
    {
        public CustomerProfileRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<CustomerProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<CustomerProfile>().FirstOrDefaultAsync(cp => cp.UserId == userId);
        }
    }
}
