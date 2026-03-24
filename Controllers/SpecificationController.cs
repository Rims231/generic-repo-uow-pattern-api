
using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.Repository;
using generic_repo_uow_pattern_api.Specification;
using Microsoft.AspNetCore.Mvc;

namespace generic_repo_uow_pattern_api.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class SpecificationController : ControllerBase
    {
        private readonly IRepository<Product> _repo;

        public SpecificationController(IRepository<Product> repo)
        {
            _repo = repo;
        }

 
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? productName = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? orderByPriceAsc = null,
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null)
        {
            var spec = new ProductSpecification(productName, minPrice, maxPrice,
                                                   orderByPriceAsc, pageNumber, pageSize);
            var results = await _repo.GetWithSpecAsync(spec);
            return Ok(results);
        }

     
        [HttpGet("products/{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var spec = new ProductSpecification(id);
            var product = await _repo.GetEntityWithSpecAsync(spec);
            return product is null ? NotFound() : Ok(product);
        }

     
        [HttpGet("products/count")]
        public async Task<IActionResult> GetProductCount(
            [FromQuery] string? productName = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            var spec = new ProductSpecification(productName, minPrice, maxPrice);
            var count = await _repo.CountWithSpecAsync(spec);
            return Ok(new { productName, minPrice, maxPrice, count });
        }
    }
}