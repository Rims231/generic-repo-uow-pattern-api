
using generic_repo_uow_pattern_api.Entity;

namespace generic_repo_uow_pattern_api.Specification
{
    
    public class ProductSpecification : BaseSpecification<Product>
    {
     
        public ProductSpecification(
            string? productName = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? orderByPriceAsc = null,
            int? pageNumber = null,
            int? pageSize = null)
        {

            if (!string.IsNullOrWhiteSpace(productName) || minPrice.HasValue || maxPrice.HasValue)
            {
                AddCriteria(p =>
                    (string.IsNullOrWhiteSpace(productName)
                        || p.ProductName.ToLower().Contains(productName.ToLower()))
                    && (!minPrice.HasValue || p.Price >= minPrice.Value)
                    && (!maxPrice.HasValue || p.Price <= maxPrice.Value));
            }

           
            if (orderByPriceAsc.HasValue)
            {
                if (orderByPriceAsc.Value)
                    AddOrderBy(p => p.Price);
                else
                    AddOrderByDescending(p => p.Price);
            }

       
            if (pageNumber.HasValue && pageSize.HasValue)
                ApplyPaging(pageNumber.Value, pageSize.Value);
        }

    
        public ProductSpecification(int id)
        {
            AddCriteria(p => p.Id == id);
        }
    }
}