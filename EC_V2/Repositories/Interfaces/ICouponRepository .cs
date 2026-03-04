using EC_V2.Models;

namespace EC_V2.Repositories.Interfaces
{
    public interface ICouponRepository: IGenericRepository<Coupon>
    {
        Task<Coupon?> GetByCode(string code);

    }
}
