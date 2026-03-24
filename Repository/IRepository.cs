using generic_repo_uow_pattern_api.Data;
using System.Linq.Expressions;

namespace generic_repo_uow_pattern_api.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);

        // ── Expression-based ──────────────────────────────────────────────────
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>>? predicate, int pageNumber, int pageSize);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
        Task<IEnumerable<T>> FindWithOrderAsync<TKey>(
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, TKey>> orderBy,
            bool ascending = true);

        // ── Specification-based ───────────────────────────────────────────────
        Task<IEnumerable<T>> GetWithSpecAsync(ISpecification<T> spec);
        Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);
        Task<int> CountWithSpecAsync(ISpecification<T> spec);

        void SetDbContext(MyDbContext dbContext);
    }
}