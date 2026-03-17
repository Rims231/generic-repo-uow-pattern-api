using generic_repo_uow_pattern_api.Data;

namespace generic_repo_uow_pattern_api.Repository
{
    public interface IUnitOfWork : IDisposable
    {

        public IProductRepository ProductRepository { get; }
        public IRepository<T> GetRepository<T>() where T : class;

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}
