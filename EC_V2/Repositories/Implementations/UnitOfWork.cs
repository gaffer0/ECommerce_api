using EC_V2.Data;
using EC_V2.Repositories.Interfaces;

namespace EC_V2.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IVendorProfileRepository VendorProfile { get; private set; }
        public ICustomerProfileRepository CustomerProfile { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public IOrderRepository Order { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Product = new ProductRepository(_context);
            Category = new CategoryRepository(_context);
            VendorProfile = new VendorProfileRepository(_context);
            CustomerProfile = new CustomerProfileRepository(_context);
            Coupon = new CouponRepository(_context);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
