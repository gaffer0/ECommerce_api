using AutoMapper;
using EC_V2.Data;
using EC_V2.Dtos;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        //private readonly AppDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoriesController> _logger;
        private readonly IMapper _mapper;
        public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Getting category with id {Id}", id);
            var category = await _unitOfWork.Category.GetByIdWithParent(id);
            if (category == null)
            {
                _logger.LogWarning("Category with id {Id} not found", id);
                return NotFound("There isn't a category with this id");
            }
            return Ok(_mapper.Map<CategoryDto>(category));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.Category.GetAllWithParent();
            return Ok(_mapper.Map<List<CategoryDto>>(categories));
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCategoryDto addCategoryDto)
        {
            if (addCategoryDto == null) return BadRequest("You must insert data");

            if (addCategoryDto.ParentId.HasValue)
            {
                var parent = await _unitOfWork.Category.GetById(addCategoryDto.ParentId.Value);
                if (parent == null) return BadRequest("Parent category not found");
            }

            var category = _mapper.Map<Category>(addCategoryDto);
            await _unitOfWork.Category.Add(category);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0) return Created();
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto updateCategoryDto)
        {
            _logger.LogInformation("Updating category with id {Id}", id);
            var category = await _unitOfWork.Category.GetByIdWithParent(id);
            if (category == null) return NotFound();

            _mapper.Map(updateCategoryDto, category);
            _unitOfWork.Category.Update(category);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0) return Ok(_mapper.Map<CategoryDto>(category));
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Category.GetById(id);
            if (category == null) return NotFound();

            _unitOfWork.Category.Delete(category);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Category with id {Id} deleted successfully", id);
            return Ok("Deleted");
        }
    }
}
