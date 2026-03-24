using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace generic_repo_uow_pattern_api.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class ExpressionController : ControllerBase
    {
        private readonly IRepository<Product> _repo;

        public ExpressionController(IRepository<Product> repo)
        {
            _repo = repo;
        }

       
        [HttpGet("find")]
        public async Task<IActionResult> Find([FromQuery] decimal minPrice = 0)
        {
            Expression<Func<Product, bool>> predicate = p => p.Price >= minPrice;
            var results = await _repo.FindAsync(predicate);
            return Ok(results);
        }


        [HttpGet("first")]
        public async Task<IActionResult> First([FromQuery] string productName)
        {
            var product = await _repo.FirstOrDefaultAsync(p => p.ProductName == productName);
            return product is null ? NotFound() : Ok(product);
        }

       
        [HttpGet("exists")]
        public async Task<IActionResult> Exists([FromQuery] decimal minPrice = 0)
        {
            var exists = await _repo.AnyAsync(p => p.Price >= minPrice);
            return Ok(new { minPrice, exists });
        }

   
        [HttpGet("count")]
        public async Task<IActionResult> Count([FromQuery] decimal? minPrice = null)
        {
            var count = minPrice.HasValue
                ? await _repo.CountAsync(p => p.Price >= minPrice.Value)
                : await _repo.CountAsync();

            return Ok(new { count });
        }

      
        [HttpGet("paged")]
        public async Task<IActionResult> Paged(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] decimal? minPrice = null)
        {
            Expression<Func<Product, bool>>? predicate =
                minPrice.HasValue ? p => p.Price >= minPrice.Value : null;

            var items = await _repo.GetPagedAsync(predicate, page, size);
            return Ok(items);
        }

       
        [HttpGet("names")]
        public async Task<IActionResult> Names()
        {
            var names = await _repo.SelectAsync(p => new { p.Id, p.ProductName });
            return Ok(names);
        }

     
        [HttpGet("by-name")]
        public async Task<IActionResult> GetProductByName(
            [FromQuery] string productName,
            [FromQuery] decimal? minPrice = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return BadRequest("productName is required.");

            Expression<Func<Product, bool>> predicate = (minPrice.HasValue)
                ? p => p.ProductName.ToLower().Contains(productName.ToLower())
                       && p.Price >= minPrice.Value
                : p => p.ProductName.ToLower().Contains(productName.ToLower());

            var results = await _repo.FindAsync(predicate);
            return Ok(results);
        }

     
        [HttpGet("count-by-name")]
        public async Task<IActionResult> GetProductCountWithName(
            [FromQuery] string productName,
            [FromQuery] decimal? minPrice = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return BadRequest("productName is required.");

            Expression<Func<Product, bool>> predicate = (minPrice.HasValue)
                ? p => p.ProductName.ToLower().Contains(productName.ToLower())
                       && p.Price >= minPrice.Value
                : p => p.ProductName.ToLower().Contains(productName.ToLower());

            var count = await _repo.CountAsync(predicate);
            return Ok(new { productName, minPrice, count });
        }

    
        [HttpGet("sorted")]
        public async Task<IActionResult> Sorted(
            [FromQuery] bool ascending = true,
            [FromQuery] decimal? minPrice = null)
        {
            Expression<Func<Product, bool>>? predicate =
                minPrice.HasValue ? p => p.Price >= minPrice.Value : null;

            var results = await _repo.FindWithOrderAsync(
                predicate,
                orderBy: p => p.Price,
                ascending: ascending);

            return Ok(results);
        }
    }
}