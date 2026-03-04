//using System.Collections.Concurrent;
//using EC_V2.Dtos;
//using EC_V2.Models;
//using EC_V2.Repositories.Interfaces;
//using EC_V2.Services.Interfaces;

//namespace EC_V2.Services.Implementations
//{
//    public class CartService : ICartService
//    {
//        private readonly ILogger<CartService> _logger;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly ConcurrentDictionary<string, List<CartItem>> _carts = new();

//        public CartService(ILogger<CartService> logger, IUnitOfWork unitOfWork)
//        {
//            _logger = logger;
//            _unitOfWork = unitOfWork;
//        }

//        public CartDto GetCart(string userId)
//        {
//            _logger.LogInformation("Retrieving cart for user {UserId}", userId);
//            var items = _carts.GetOrAdd(userId, new List<CartItem>());
//            return new CartDto { Items = items };
//        }
//        public async Task<ServiceResult<bool>> AddToCart(string userId, AddToCartDto dto)
//        {
//            _logger.LogInformation("Adding product {ProductId} to cart for user {UserId}", dto.ProductId, userId);
//            var product = await _unitOfWork.Product.GetById(dto.ProductId);
//            if (product == null)
//            {
//                _logger.LogWarning("Product {ProductId} not found", dto.ProductId);
//                return new ServiceResult<bool> { Success = false, Error = "Product not found" };
//            }
//            var cart = _carts.GetOrAdd(userId, new List<CartItem>());

//            var existing = cart.FirstOrDefault(i => i.ProductId == dto.ProductId);
//            if (existing != null)
//            {
//                existing.Quantity += dto.Quantity;
//            }
//            else
//            {
//                cart.Add(new CartItem
//                {
//                    ProductId = product.Id,
//                    ProductName = product.Name,
//                    Price = product.Price,
//                    Quantity = dto.Quantity
//                });
//            }
//            return new ServiceResult<bool> { Success = true, Data = true };

//        }
//        public async Task<ServiceResult<bool>> RemoveFromCart(string userId, int productId)
//        {
//            _logger.LogInformation("Removing product {ProductId} from cart for user {UserId}", productId, userId);
//            var cart = _carts.GetOrAdd(userId, new List<CartItem>());
//            var item = cart.FirstOrDefault(i => i.ProductId == productId);
//            if (item == null)
//            {
//                _logger.LogWarning("Product {ProductId} not found in cart for user {UserId}", productId, userId);
//                return new ServiceResult<bool> { Success = false, Error = "Product not in cart" };
//            }
//            cart.Remove(item);
//            return new ServiceResult<bool> { Success = true, Data = true };
//        }
//        public async Task<ServiceResult<bool>> UpdateQuantity(string userId, int productId, int quantity)
//        {
//            _logger.LogInformation("Updating quantity of product {ProductId} to {Quantity} in cart for user {UserId}", productId, quantity, userId);
//            var cart = _carts.GetOrAdd(userId, new List<CartItem>());
//            var item = cart.FirstOrDefault(i => i.ProductId == productId);
//            if (item == null)
//            {
//                _logger.LogWarning("Product {ProductId} not found in cart for user {UserId}", productId, userId);
//                return new ServiceResult<bool> { Success = false, Error = "Product not in cart" };
//            }
//            if (quantity <= 0)
//            {
//                cart.Remove(item);
//            }
//            else
//            {
//                item.Quantity = quantity;
//            }
//            return new ServiceResult<bool> { Success = true, Data = true };
//        }
//        public void ClearCart(string userId)
//        {
//            _logger.LogInformation("Clearing cart for user {UserId}", userId);
//            _carts.TryRemove(userId, out _);

//        }
//    }
//}
