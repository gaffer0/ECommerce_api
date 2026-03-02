using AutoMapper;
using EC_V2.Dtos;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EC_V2.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IUnitOfWork unitOfWork, IMapper Mapper, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = Mapper;
            _logger = logger;
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    _logger.LogInformation("Getting all products");
        //    var products = await _unitOfWork.Product.GetAllWithCategories();
        //    _logger.LogInformation("Returned {Count} products", products.Count());
        //    return Ok(_mapper.Map<List<ProductDto>>(products));
        //}
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
        {
            _logger.LogInformation("Getting products with query: {@Query}", query);
            var pagedProducts = await _unitOfWork.Product.GetPagedProducts(query);
            var productDtos = _mapper.Map<List<ProductDto>>(pagedProducts.Items);
            return Ok(new PagedResult<ProductDto>
            {
                Items = productDtos,
                NextCursor = pagedProducts.NextCursor,
                HasMore = pagedProducts.HasMore
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Getting product with id {Id}", id);
            var product = await _unitOfWork.Product.GetByIdWithCategories(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id {Id} not found", id);
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddProductDto addProductDto)
        {
            _logger.LogInformation("Creating product {Name}", addProductDto.Name);

            if (addProductDto.CategoryIds == null || !addProductDto.CategoryIds.Any())
            {
                _logger.LogWarning("No categories provided");
                return BadRequest("At least one category must be provided");
            }

            var categories = await _unitOfWork.Category.GetByIds(addProductDto.CategoryIds);
            if (categories.Count != addProductDto.CategoryIds.Count)
            {
                _logger.LogWarning("Some category IDs were not found");
                return BadRequest("One or more category IDs are invalid");
            }

            var product = _mapper.Map<Product>(addProductDto);
            product.Categories = categories;

            await _unitOfWork.Product.Add(product);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Product {Name} created with id {Id}", product.Name, product.Id);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, _mapper.Map<ProductDto>(product));
            }

            _logger.LogError("Failed to create product {Name}", addProductDto.Name);
            return StatusCode(500, "An error occurred while creating the product");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AddProductDto updateProductDto)
        {
            _logger.LogInformation("Updating product with id {Id}", id);
            var product = await _unitOfWork.Product.GetByIdWithCategories(id);
            if (product == null) return NotFound();

            _mapper.Map(updateProductDto, product);

            if (updateProductDto.CategoryIds != null && updateProductDto.CategoryIds.Any())
            {
                var categories = await _unitOfWork.Category.GetByIds(updateProductDto.CategoryIds);
                if (categories.Count != updateProductDto.CategoryIds.Count)
                {
                    _logger.LogWarning("Some category IDs were not found for product update {Id}", id);
                    return BadRequest("One or more category IDs are invalid");
                }
                product.Categories = categories;
            }

            _unitOfWork.Product.Update(product);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Product {Id} updated successfully", id);
                return Ok(_mapper.Map<ProductDto>(product));
            }

            _logger.LogError("Failed to update product {Id}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Product.GetById(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id {Id} not found", id);
                return NotFound();
            }

            _unitOfWork.Product.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Product {Id} deleted successfully", id);
            return Ok("Product deleted successfully");
        }

    }
}
