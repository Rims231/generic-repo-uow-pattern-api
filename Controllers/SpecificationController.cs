
using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.Repository;
using generic_repo_uow_pattern_api.Specification;
using Microsoft.AspNetCore.Mvc;

namespace generic_repo_uow_pattern_api.Controllers
{
    /// <summary>
    /// Exposes the specification-based repository methods for Products.
    /// All filtering, ordering, and paging is encapsulated inside ProductSpecification.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SpecificationController : ControllerBase
    {
        private readonly IRepository<Product> _repo;

        public SpecificationController(IRepository<Product> repo)
        {
            _repo = repo;
        }

        // ── GET api/specification/products ────────────────────────────────────
        /// <summary>
        /// GetWithSpecAsync – returns a filtered, ordered, paged list of products.
        /// All parameters are optional and can be combined freely.
        /// </summary>
        /// <param name="productName">Partial name search (case-insensitive).</param>
        /// <param name="minPrice">Lower price bound (inclusive).</param>
        /// <param name="maxPrice">Upper price bound (inclusive).</param>
        /// <param name="orderByPriceAsc">true = cheapest first | false = most expensive first</param>
        /// <param name="pageNumber">1-based page index (requires pageSize).</param>
        /// <param name="pageSize">Number of results per page (requires pageNumber).</param>
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

        // ── GET api/specification/products/{id} ───────────────────────────────
        /// <summary>
        /// GetEntityWithSpecAsync – returns a single product by Id.
        /// </summary>
        [HttpGet("products/{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var spec = new ProductSpecification(id);
            var product = await _repo.GetEntityWithSpecAsync(spec);
            return product is null ? NotFound() : Ok(product);
        }

        // ── GET api/specification/products/count ──────────────────────────────
        /// <summary>
        /// CountWithSpecAsync – returns the count of products matching the spec.
        /// Uses the same optional parameters as the list endpoint.
        /// </summary>
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