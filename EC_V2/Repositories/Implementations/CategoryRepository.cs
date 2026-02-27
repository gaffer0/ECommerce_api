using EC_V2.Data;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Repositories.Implementations
{
    public class CategoryRepository:GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Category>> GetByIds(List<int> ids)
        {
            return await _context.Set<Category>().Where(c => ids.Contains(c.Id)).ToListAsync();
        }
        public async Task<IEnumerable<Category>> GetAllWithParent()
        {
            return await _context.Set<Category>()
                .Include(c => c.Parent)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdWithParent(int id)
        {
            return await _context.Set<Category>()
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

    }
}
