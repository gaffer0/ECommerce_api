using EC_V2.Dtos;
using EC_V2.Models;

namespace EC_V2.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategories();
        Task<Product?> GetByIdWithCategories(int id);
        Task<List<Product>> GetByIds(List<int> ids);
        Task<PagedResult<Product>> GetPagedProducts(ProductQueryDto query);
    }
}
