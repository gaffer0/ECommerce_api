using EC_V2.Dtos;
using EC_V2.Dtos.OrderDtos;
using EC_V2.Models.Enums;

namespace EC_V2.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderDto>> CreateOrder(string customerId, CreateOrderDto dto);
        Task<ServiceResult<OrderDto>> GetOrderById(int orderId, string userId, string role);
        Task<ServiceResult<List<OrderDto>>> GetCustomerOrders(string customerId);
        Task<ServiceResult<List<OrderDto>>> GetVendorOrders(string vendorId);
        Task<ServiceResult<List<OrderDto>>> GetAllOrders(); // Admin only
        Task<ServiceResult<bool>> UpdateOrderStatus(int orderId, OrderStatus status, string userId, string role);
        Task<ServiceResult<bool>> CancelOrder(int orderId, string customerId);
    }
}
