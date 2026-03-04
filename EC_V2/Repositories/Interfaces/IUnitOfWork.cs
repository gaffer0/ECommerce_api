namespace EC_V2.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
        IVendorProfileRepository VendorProfile { get; }
        ICustomerProfileRepository CustomerProfile { get; }
        ICouponRepository Coupon { get; }
        IOrderRepository Order { get; }


        Task<int> SaveChangesAsync();
    }
}
