using EC_V2.Data;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Repositories.Implementations
{
    public class CouponRepository:GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context) { }
        public async Task<Coupon?> GetByCode(string code)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
        }
            

    }    
}
