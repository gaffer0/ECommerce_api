using System.Security.Claims;
using EC_V2.Dtos;
using EC_V2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EC_V2.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;
        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            _logger.LogInformation("Getting cart for user {UserId}", userId);
            var cart = _cartService.GetCart(userId);
            return Ok(cart);

        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            _logger.LogInformation("Adding product {ProductId} to cart for user {UserId}", dto.ProductId, userId);
            var result = await _cartService.AddToCart(userId, dto);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Error);

        }
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            _logger.LogInformation("Removing product {ProductId} from cart for user {UserId}", productId, userId);
            var result = await _cartService.RemoveFromCart(userId, productId);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Error);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            _logger.LogInformation("Updating quantity of product {ProductId} to {Quantity} for user {UserId}", productId, quantity, userId);
            var result = await _cartService.UpdateQuantity(userId, productId, quantity);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Error);
        }
        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            _logger.LogInformation("Clearing cart for user {UserId}", userId);
            _cartService.ClearCart(userId);
            return Ok();
        }
    }
}
