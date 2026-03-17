using generic_repo_uow_pattern_api.Data;
using generic_repo_uow_pattern_api.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace generic_repo_uow_pattern_api.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _myDbContext;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public IProductRepository ProductRepository {  get;  }
        public UnitOfWork(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
            _repositories = new Dictionary<Type, object>();
            ProductRepository = new ProductRepository(_myDbContext);
        }

        //public async Task BeginTransactionAsync()
        //{
        //    var transaction = _myDbContext.Database.BeginTransactionAsync();
        //}

        public async Task BeginTransactionAsync()
        {
            _transaction = await _myDbContext.Database.BeginTransactionAsync(); // ✅
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
                // Dispose managed resources
                _myDbContext?.Dispose();
            }

            // Free unmanaged resources here if any

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
            if (_transaction == null) return; // ✅ guard against null

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
    }
}
