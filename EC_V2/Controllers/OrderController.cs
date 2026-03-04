using System.Security.Claims;
using EC_V2.Dtos.OrderDtos;
using EC_V2.Models.Enums;
using EC_V2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EC_V2.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        private string? GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value;



        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();
            _logger.LogInformation("Creating order for user {UserId}", GetUserId());
            var result = await _orderService.CreateOrder(userId, dto);
            if (!result.Success)
                return BadRequest(result.Error);
            return Ok(result.Data);

        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerOrders()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();
            _logger.LogInformation("Getting orders for user {UserId} ", userId);
            var result = await _orderService.GetCustomerOrders(userId);
            if (!result.Success)
                return BadRequest(result.Error);
            return Ok(result.Data);


        }

        // Vendor orders
        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor")]
        public async Task<IActionResult> GetVendorOrders() {
        
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();
            _logger.LogInformation("Getting orders for vendor {VendorId} ", userId);
            var result = await _orderService.GetVendorOrders(userId);
            if (!result.Success)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }

        // Admin all orders
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders() {
        
            _logger.LogInformation("Getting all orders for admin");
            var result = await _orderService.GetAllOrders();
            if (!result.Success)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }

        // Get by ID - any role
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id) { 
        
                var userId = GetUserId();
                var role = GetUserRole();
                if (userId == null || role == null)
                    return Unauthorized();
                _logger.LogInformation("Getting order {OrderId} for user {UserId} with role {Role}", id, userId, role);
                var result = await _orderService.GetOrderById(id, userId, role);
                if (!result.Success)
                    return BadRequest(result.Error);
                return Ok(result.Data);
        }

        // Update status - Vendor/Admin
        [Authorize(Roles = "Vendor,Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus status) { 
        
            var userId = GetUserId();
            var role = GetUserRole();
            if (userId == null || role == null)
                return Unauthorized();
            _logger.LogInformation("Updating status of order {OrderId} to {Status} by user {UserId} with role {Role}", id, status, userId, role);
            var result = await _orderService.UpdateOrderStatus(id, status, userId, role);
            if (!result.Success)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }

        // Cancel - Customer only
        [Authorize(Roles = "Customer")]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id) {
        
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();
            _logger.LogInformation("Cancelling order {OrderId} by user {UserId}", id, userId);
            var result = await _orderService.CancelOrder(id, userId);
            if (!result.Success)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
    }
}
