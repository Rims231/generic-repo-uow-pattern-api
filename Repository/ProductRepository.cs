using generic_repo_uow_pattern_api.Data;
using generic_repo_uow_pattern_api.Entity;
using Microsoft.EntityFrameworkCore;

namespace generic_repo_uow_pattern_api.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(MyDbContext myDbContext) : base(myDbContext)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string productName)
        {
          return  await _dbSet.Where(p => p.ProductName.Contains(productName)).ToListAsync();
             return await _dbSet.Where(p => p.ProductName.Contains(productName)).ToListAsync();

        }

      

        
    }
}
