using EC_V2.Dtos;

namespace EC_V2.Services.Interfaces
{
    public interface ICartService
    {
        CartDto GetCart(string userId);
        Task <ServiceResult<bool>> AddToCart(string userId, AddToCartDto dto);
        Task <ServiceResult<bool>> RemoveFromCart(string userId, int productId);
        Task <ServiceResult<bool>> UpdateQuantity(string userId, int productId, int quantity);
        void ClearCart(string userId);
    }
}
