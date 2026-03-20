using AutoMapper;
using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.Repository;
using generic_repo_uow_pattern_api.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace generic_repo_uow_pattern_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductWithGenericController : ControllerBase
    {
        private readonly IProductRepository _productrepository; // ✅ changed
        private readonly IMapper _mapper;

        public ProductWithGenericController(IProductRepository productrepository, IMapper mapper) // ✅ changed
        {
            _productrepository = productrepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productrepository.GetAllAsync();
            var mappedProducts = _mapper.Map<IEnumerable<ProductRequest>>(products);
            return Ok(mappedProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productrepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var response = _mapper.Map<ProductRequest>(product);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductRequest product)
        {
            var productEntity = _mapper.Map<Product>(product);
            var createdProduct = await _productrepository.AddAsync(productEntity);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductRequest product)
        {
            var productEntity = await _productrepository.GetByIdAsync(id);
            if (productEntity == null) return NotFound();

            _mapper.Map(product, productEntity);
            await _productrepository.UpdateAsync(productEntity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var productEntity = await _productrepository.GetByIdAsync(id);
            if (productEntity == null) return NotFound();

            await _productrepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetProductsByPaging(
            [FromQuery] decimal minPrice = 0,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = "")
        {
            var result = await _productrepository.GetProductsByPaging(minPrice, pageNumber, pageSize, searchTerm); // ✅

            var mappedResult = _mapper.Map<IEnumerable<ProductRequest>>(result);

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };

            return Ok(new { data = mappedResult, pagination = metadata });
        }
    }
}