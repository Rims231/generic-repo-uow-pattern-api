using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace generic_repo_uow_pattern_api.Controllers
{
    /// <summary>
    /// Demonstrates every expression-based method on IRepository&lt;T&gt;.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExpressionController : ControllerBase
    {
        private readonly IRepository<Product> _repo;

        public ExpressionController(IRepository<Product> repo)
        {
            _repo = repo;
        }

        // ── GET api/expression/find?minPrice=10 ───────────────────────────────
        /// <summary>FindAsync – returns all products whose price >= minPrice.</summary>
        [HttpGet("find")]
        public async Task<IActionResult> Find([FromQuery] decimal minPrice = 0)
        {
            Expression<Func<Product, bool>> predicate = p => p.Price >= minPrice;
            var results = await _repo.FindAsync(predicate);
            return Ok(results);
        }

        // ── GET api/expression/first?productName=Widget ───────────────────────
        /// <summary>FirstOrDefaultAsync – returns the first product with a matching name.</summary>
        [HttpGet("first")]
        public async Task<IActionResult> First([FromQuery] string productName)
        {
            var product = await _repo.FirstOrDefaultAsync(p => p.ProductName == productName);
            return product is null ? NotFound() : Ok(product);
        }

        // ── GET api/expression/exists?minPrice=5 ──────────────────────────────
        /// <summary>AnyAsync – checks whether any product exists above the given price.</summary>
        [HttpGet("exists")]
        public async Task<IActionResult> Exists([FromQuery] decimal minPrice = 0)
        {
            var exists = await _repo.AnyAsync(p => p.Price >= minPrice);
            return Ok(new { minPrice, exists });
        }

        // ── GET api/expression/count?minPrice=5 ───────────────────────────────
        /// <summary>
        /// CountAsync – total count when no query string is supplied;
        /// filtered count when minPrice is provided.
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> Count([FromQuery] decimal? minPrice = null)
        {
            var count = minPrice.HasValue
                ? await _repo.CountAsync(p => p.Price >= minPrice.Value)
                : await _repo.CountAsync();

            return Ok(new { count });
        }

        // ── GET api/expression/paged?page=1&size=10&minPrice=0 ───────────────
        /// <summary>GetPagedAsync – paged list of products, optionally filtered by minPrice.</summary>
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

        // ── GET api/expression/names ──────────────────────────────────────────
        /// <summary>SelectAsync – projects every product to just its Id + ProductName.</summary>
        [HttpGet("names")]
        public async Task<IActionResult> Names()
        {
            var names = await _repo.SelectAsync(p => new { p.Id, p.ProductName });
            return Ok(names);
        }

        // ── GET api/expression/by-name?productName=Widget&minPrice=5 ─────────
        /// <summary>
        /// GetProductByName – searches by ProductName (partial, case-insensitive)
        /// and optionally filters by a minimum Price at the same time.
        /// </summary>
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

        // ── GET api/expression/count-by-name?productName=Widget&minPrice=5 ───
        /// <summary>
        /// GetProductCountWithName – returns the count of products whose name
        /// contains the search term, with an optional minimum Price filter.
        /// </summary>
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

        // ── GET api/expression/sorted?ascending=true&minPrice=0 ──────────────
        /// <summary>
        /// FindWithOrderAsync – products filtered by optional minPrice,
        /// sorted by Price ascending or descending.
        /// </summary>
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