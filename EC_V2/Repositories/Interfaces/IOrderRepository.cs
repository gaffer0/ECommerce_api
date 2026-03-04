using EC_V2.Models;

namespace EC_V2.Repositories.Interfaces
{
    public interface IOrderRepository: IGenericRepository<Order>
    {
            Task<IEnumerable<Order>> GetCustomerOrders(string customerId);
            Task<IEnumerable<Order>> GetVendorOrders(string vendorId);
             Task<IEnumerable<Order>> GetOrdersAsync();
            

    }
}
