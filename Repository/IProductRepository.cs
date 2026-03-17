using generic_repo_uow_pattern_api.Entity;

namespace generic_repo_uow_pattern_api.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByName (string productName);
    }
}
