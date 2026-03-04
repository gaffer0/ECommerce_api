using EC_V2.Data;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Order>> GetCustomerOrders(string customerId)
        {
            return await _context.Set<Order>()
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetVendorOrders(string vendorId)
        {
            return await _context.Set<Order>()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.Product.VendorId == vendorId))
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Set<Order>()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();
        }
    }
}
