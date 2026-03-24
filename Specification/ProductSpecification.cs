
using generic_repo_uow_pattern_api.Entity;

namespace generic_repo_uow_pattern_api.Specification
{
    /// <summary>
    /// Concrete specification for querying Products.
    /// Supports name search, price range, ordering, and paging — all optional.
    /// </summary>
    public class ProductSpecification : BaseSpecification<Product>
    {
        /// <summary>
        /// Full-featured constructor: filter + order + page in one spec.
        /// </summary>
        /// <param name="productName">Partial, case-insensitive name search (null = no filter).</param>
        /// <param name="minPrice">Minimum price inclusive (null = no lower bound).</param>
        /// <param name="maxPrice">Maximum price inclusive (null = no upper bound).</param>
        /// <param name="orderByPriceAsc">True = cheapest first, False = most expensive first, null = no ordering.</param>
        /// <param name="pageNumber">1-based page number (null = no paging).</param>
        /// <param name="pageSize">Page size (null = no paging).</param>
        public ProductSpecification(
            string? productName = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? orderByPriceAsc = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            // ── Criteria ──────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(productName) || minPrice.HasValue || maxPrice.HasValue)
            {
                AddCriteria(p =>
                    (string.IsNullOrWhiteSpace(productName)
                        || p.ProductName.ToLower().Contains(productName.ToLower()))
                    && (!minPrice.HasValue || p.Price >= minPrice.Value)
                    && (!maxPrice.HasValue || p.Price <= maxPrice.Value));
            }

            // ── Ordering ──────────────────────────────────────────────────────
            if (orderByPriceAsc.HasValue)
            {
                if (orderByPriceAsc.Value)
                    AddOrderBy(p => p.Price);
                else
                    AddOrderByDescending(p => p.Price);
            }

            // ── Paging ────────────────────────────────────────────────────────
            if (pageNumber.HasValue && pageSize.HasValue)
                ApplyPaging(pageNumber.Value, pageSize.Value);
        }

        /// <summary>Single-product lookup by Id.</summary>
        public ProductSpecification(int id)
        {
            AddCriteria(p => p.Id == id);
        }
    }
}