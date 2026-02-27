using EC_V2.Models;

namespace EC_V2.Repositories.Interfaces
{
    public interface ICategoryRepository:IGenericRepository<Category>
    {
        Task<List<Category>> GetByIds(List<int> ids);
        Task<IEnumerable<Category>> GetAllWithParent();
        Task<Category?> GetByIdWithParent(int id);
    }
}
