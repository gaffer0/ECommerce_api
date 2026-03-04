using AutoMapper;
using EC_V2.Dtos;
using EC_V2.Dtos.OrderDtos;
using EC_V2.Models;
using EC_V2.Models.Enums;
using EC_V2.Repositories.Interfaces;
using EC_V2.Services.Interfaces;

namespace EC_V2.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResult<OrderDto>> CreateOrder(string customerId, CreateOrderDto dto)
        {
            // Get all product IDs from the order
            var productIds = dto.Items.Select(i => i.ProductId).ToList();

            // Fetch all products in ONE query
            var products = await _unitOfWork.Product.GetByIds(productIds);

            // Check if all products exist
            if (products.Count != productIds.Count)
                return new ServiceResult<OrderDto> { Success = false, Error = "One or more products not found" };

            // Check stock for each
            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (product.Stock < item.Quantity)
                    return new ServiceResult<OrderDto> { Success = false, Error = $"Insufficient stock for {product.Name}" };
            }
            //calulate totals
            decimal subTotal = 0;
            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                subTotal += product.Price * item.Quantity;
            }
            //Tax

            decimal taxAmount = subTotal * 0.14m; // 14% tax
            //cupons
            decimal discountAmount = 0;
            Coupon? coupon = null;

            if (!string.IsNullOrEmpty(dto.CouponCode))
            {
                coupon = await _unitOfWork.Coupon.GetByCode(dto.CouponCode);

                if (coupon == null)
                    return new ServiceResult<OrderDto> { Success = false, Error = "Invalid coupon code" };

                if (!coupon.IsActive)
                    return new ServiceResult<OrderDto> { Success = false, Error = "Coupon is not active" };

                if (coupon.ExpiryDate < DateTime.UtcNow)
                    return new ServiceResult<OrderDto> { Success = false, Error = "Coupon has expired" };

                if (subTotal < coupon.MinOrderAmount)
                    return new ServiceResult<OrderDto> { Success = false, Error = $"Minimum order amount is {coupon.MinOrderAmount}" };

                if (coupon.MaxUses != null && coupon.UsedCount >= coupon.MaxUses)
                    return new ServiceResult<OrderDto> { Success = false, Error = "Coupon has reached maximum uses" };

                // Calculate discount
                discountAmount = coupon.Type == DiscountType.Percentage
                    ? subTotal * (coupon.Value / 100)
                    : coupon.Value;
            }
            // Grand total
            decimal grandTotal = subTotal + taxAmount - discountAmount;
            // Create order
            var order = new Order
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ShippingAddress = dto.ShippingAddress,
                SubTotal = subTotal,
                TaxAmount = taxAmount,
                DiscountAmount = discountAmount,
                GrandTotal = grandTotal,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = products.First(p => p.Id == i.ProductId).Name, // ← add this
                    Quantity = i.Quantity,
                    UnitPrice = products.First(p => p.Id == i.ProductId).Price
                }).ToList()
            };

            foreach (var item in order.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                product.Stock -= item.Quantity; // Reduce stock
                _unitOfWork.Product.Update(product);
            }

            if (coupon != null)
            {
                coupon.UsedCount++;
                _unitOfWork.Coupon.Update(coupon);
            }

            await _unitOfWork.Order.Add(order);
            await _unitOfWork.SaveChangesAsync();
            return new ServiceResult<OrderDto>
            {
                Success = true,
                Data = _mapper.Map<OrderDto>(order)
               
            };
        }
        public async Task<ServiceResult<OrderDto>> GetOrderById(int orderId, string userId, string role)
        {
            var order = await _unitOfWork.Order.GetById(orderId);
            if (order == null)
                return new ServiceResult<OrderDto> { Success = false, Error = "Order not found" };
            if (role == "Customer" && order.CustomerId != userId)
                return new ServiceResult<OrderDto> { Success = false, Error = "Unauthorized" };
            // For vendors and admins, you might want to add additional checks here
            return new ServiceResult<OrderDto>
            {
                Success = true,
                Data = _mapper.Map<OrderDto>(order)

            };
        }

        public async Task<ServiceResult<List<OrderDto>>> GetCustomerOrders(string customerId)
        {
            var orders = await _unitOfWork.Order.GetCustomerOrders(customerId);
            return new ServiceResult<List<OrderDto>>
            {
                Success = true,
                Data = _mapper.Map<List<OrderDto>>(orders)
            };


        }
        public async Task<ServiceResult<List<OrderDto>>> GetVendorOrders(string vendorId)
        {
            var orders = await _unitOfWork.Order.GetVendorOrders(vendorId);
            return new ServiceResult<List<OrderDto>>
            {
                Success = true,
                Data = _mapper.Map<List<OrderDto>>(orders)
            };
        }
        public async Task<ServiceResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _unitOfWork.Order.GetOrdersAsync();
            return new ServiceResult<List<OrderDto>>
            {
                Success = true,
                Data = _mapper.Map<List<OrderDto>>(orders)
            };
        }
        public async Task<ServiceResult<bool>> UpdateOrderStatus(int orderId, OrderStatus status, string userId, string role)
        {
            var order = await _unitOfWork.Order.GetById(orderId);
            if (order == null)
                return new ServiceResult<bool> { Success = false, Error = "Order not found" };
            if (role == "Customer")
                return new ServiceResult<bool> { Success = false, Error = "Unauthorized" };
            // For vendors, you might want to check if the order contains their products
            order.Status = status;
            _unitOfWork.Order.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return new ServiceResult<bool> { Success = true, Data = true };
        }
        public async Task<ServiceResult<bool>> CancelOrder(int orderId, string customerId)
        {
            var order = await _unitOfWork.Order.GetById(orderId);
            if (order == null)
                return new ServiceResult<bool> { Success = false, Error = "Order not found" };
            if (order.CustomerId != customerId)
                return new ServiceResult<bool> { Success = false, Error = "Unauthorized" };
            if (order.Status != OrderStatus.Pending)
                return new ServiceResult<bool> { Success = false, Error = "Only pending orders can be cancelled" };
            order.Status = OrderStatus.Cancelled;
            _unitOfWork.Order.Update(order);

            // Restore stock
            var productIds = order.Items.Select(i => i.ProductId).ToList();
            var products = await _unitOfWork.Product.GetByIds(productIds);
            foreach (var item in order.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                product.Stock += item.Quantity;
                _unitOfWork.Product.Update(product);
            }
            await _unitOfWork.SaveChangesAsync();
            return new ServiceResult<bool> { Success = true, Data = true };
        }
    }
}