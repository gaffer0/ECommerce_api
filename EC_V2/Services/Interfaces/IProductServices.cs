using EC_V2.Dtos;

namespace EC_V2.Services.Interfaces
{
    public interface IProductServices
    {
        Task<PagedResult<ProductDto>> GetProducts(ProductQueryDto query);
    }
}
