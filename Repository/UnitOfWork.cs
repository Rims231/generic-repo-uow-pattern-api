using generic_repo_uow_pattern_api.Data;
using generic_repo_uow_pattern_api.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace generic_repo_uow_pattern_api.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _myDbContext;
        private readonly IServiceProvider _serviceProvider;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public IProductRepository ProductRepository { get; }
        public UnitOfWork(MyDbContext myDbContext, IServiceProvider serviceProvider)
        {
            _myDbContext = myDbContext;
            _serviceProvider = serviceProvider;
            _repositories = new Dictionary<Type, object>();
            ProductRepository = new ProductRepository(_myDbContext);
        }



        public async Task BeginTransactionAsync()
        {
            _transaction = await _myDbContext.Database.BeginTransactionAsync();
        }



        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {

                _myDbContext?.Dispose();
            }


            _disposed = true;
        }


        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IRepository<T>)_repositories[typeof(T)];
            }

            var repository = new Repository<T>(_myDbContext);
            _repositories[typeof(T)] = repository;
            return repository;
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null) return;

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            try
            {
                await _transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                throw new Exception("Transaction failed. Rolled back.", ex);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _myDbContext.SaveChangesAsync();
        }


        TRepository IUnitOfWork.GetRepository<TRepository, TEntity>()
    where TRepository : class
    where TEntity : class
        {
            var repository = _serviceProvider.GetService<TRepository>();
            if (repository == null)
            {
                throw new InvalidOperationException($"Repository of type {typeof(TRepository).Name} is not registered.");
            }

            if (repository is IRepository<TEntity> genericRepository)
            {
                genericRepository.SetDbContext(_myDbContext);
            }
            else
            {
                throw new InvalidOperationException($"Repository of type {typeof(TRepository).Name} does not implement IRepository<{typeof(TEntity).Name}>.");
            }

            return repository;
        }

    }
}