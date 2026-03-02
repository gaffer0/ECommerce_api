using AutoMapper;
using EC_V2.Dtos;
using EC_V2.Repositories.Interfaces;
using EC_V2.Services.Interfaces;

namespace EC_V2.Services.Implementations
{
    public class ProductServices : IProductServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public async Task<PagedResult<ProductDto>> GetProducts(ProductQueryDto query)
        {
            var pagedProducts = await _unitOfWork.Product.GetPagedProducts(query);
            var productDtos = _mapper.Map<List<ProductDto>>(pagedProducts.Items);
            return new PagedResult<ProductDto>
            {
                Items = productDtos,
                NextCursor = pagedProducts.NextCursor,
                HasMore = pagedProducts.HasMore
            };
        }
    }
}
