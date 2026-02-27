using EC_V2.Data;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Repositories.Implementations
{
    public class ProductRepository:GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Product>> GetAllWithCategories()
        {
            return await _context.Set<Product>()
                .Include(p => p.Categories)
                .ThenInclude(c => c.Parent)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdWithCategories(int id)
        {
            return await _context.Set<Product>()
                .Include(p => p.Categories)
                .ThenInclude(c => c.Parent)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
