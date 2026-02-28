using EC_V2.Models;

namespace EC_V2.Repositories.Interfaces
{
    public interface ICustomerProfileRepository : IGenericRepository<CustomerProfile>
    {
        Task<CustomerProfile?> GetByUserIdAsync(string userId);
    }
}
