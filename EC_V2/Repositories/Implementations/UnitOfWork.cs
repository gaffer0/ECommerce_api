using EC_V2.Data;
using EC_V2.Repositories.Interfaces;

namespace EC_V2.Repositories.Implementations
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Product = new ProductRepository(_context);
            Category = new CategoryRepository(_context);
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
