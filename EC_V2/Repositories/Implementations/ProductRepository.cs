using System.Text;
using System.Text.Json;
using EC_V2.Data;
using EC_V2.Dtos;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Repositories.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private string EncodeCursor(int id)
        {
            var json = JsonSerializer.Serialize(new { id });
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private int DecodeCursor(string cursor)
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var decoded = JsonSerializer.Deserialize<JsonElement>(json);
            return decoded.GetProperty("id").GetInt32();
        }
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
        public async Task<List<Product>> GetByIds(List<int> ids)
        {
            return await _context.Set<Product>()
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<Product?> GetByIdWithCategories(int id)
        {
            return await _context.Set<Product>()
                .Include(p => p.Categories)
                .ThenInclude(c => c.Parent)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<PagedResult<Product>> GetPagedProducts(ProductQueryDto query)
        {
            var productsQuery = _context.Set<Product>()
                 .Include(p => p.Categories)
                 .ThenInclude(c => c.Parent)
                 .AsQueryable();

            if (!string.IsNullOrEmpty(query.Search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(query.Search) || p.Description.Contains(query.Search));
            }
            if (query.CategoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Categories.Any(c => c.Id == query.CategoryId.Value));
            }
            if (query.MinPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice.Value);
            }
            if (query.MaxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(query.Cursor))
            {
                var lastId = DecodeCursor(query.Cursor);
                productsQuery = productsQuery.Where(p => p.Id > lastId);
            }

            productsQuery = query.SortBy switch
            {
                "price" => query.SortOrder == "asc"
                    ? productsQuery.OrderBy(p => p.Price)
                    : productsQuery.OrderByDescending(p => p.Price),
                "name" => query.SortOrder == "asc"
                    ? productsQuery.OrderBy(p => p.Name)
                    : productsQuery.OrderByDescending(p => p.Name),
                _ => productsQuery.OrderBy(p => p.Id)
            };



            var items = await productsQuery.Take(query.PageSize + 1).ToListAsync();
            var hasMore = items.Count > query.PageSize;
            if (hasMore) items.RemoveAt(items.Count - 1);
            var nextCursor = hasMore ? EncodeCursor(items.Last().Id) : null;

            return new PagedResult<Product>
            {
                Items = items,
                NextCursor = nextCursor,
                HasMore = hasMore
            };

        }





    }
}
