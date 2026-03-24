using generic_repo_uow_pattern_api.Data;
using generic_repo_uow_pattern_api.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace generic_repo_uow_pattern_api.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private MyDbContext _myDbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(MyDbContext myDbContext)
        {
            _dbSet = myDbContext.Set<T>();
            _myDbContext = myDbContext;
        }

        // ── Basic CRUD ────────────────────────────────────────────────────────

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _myDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return null;

            _dbSet.Remove(entity);
            await _myDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _myDbContext.Entry(entity).State = EntityState.Modified;
            await _myDbContext.SaveChangesAsync();
            return entity;
        }

        // ── Expression-based queries ──────────────────────────────────────────

        /// <summary>Returns all entities matching the given predicate.</summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        /// <summary>Returns the first entity matching the predicate, or null.</summary>
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        /// <summary>Returns true if any entity satisfies the predicate.</summary>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);

        /// <summary>
        /// Returns the total count of entities.
        /// When a predicate is provided, counts only matching entities.
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
            => predicate is null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);

        /// <summary>
        /// Returns a paged result set. Optionally filters by predicate before paging.
        /// pageNumber is 1-based.
        /// </summary>
        public async Task<IEnumerable<T>> GetPagedAsync(
            Expression<Func<T, bool>>? predicate,
            int pageNumber,
            int pageSize)
        {
            var query = predicate is null ? _dbSet : _dbSet.Where(predicate);
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>Projects all entities using the given selector expression.</summary>
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(
            Expression<Func<T, TResult>> selector)
            => await _dbSet.Select(selector).ToListAsync();

        /// <summary>
        /// Returns entities filtered by an optional predicate and ordered
        /// by the given key selector, ascending or descending.
        /// </summary>
        public async Task<IEnumerable<T>> FindWithOrderAsync<TKey>(
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, TKey>> orderBy,
            bool ascending = true)
        {
            var query = predicate is null ? _dbSet.AsQueryable() : _dbSet.Where(predicate);
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return await query.ToListAsync();
        }

        // ── Specification-based queries ───────────────────────────────────────

        /// <summary>Returns all entities satisfying the specification.</summary>
        public async Task<IEnumerable<T>> GetWithSpecAsync(ISpecification<T> spec)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .ToListAsync();

        /// <summary>Returns the single entity satisfying the specification, or null.</summary>
        public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .FirstOrDefaultAsync();

        /// <summary>Returns the count of entities satisfying the specification.</summary>
        public async Task<int> CountWithSpecAsync(ISpecification<T> spec)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .CountAsync();

        // ── Infrastructure ────────────────────────────────────────────────────

        public void SetDbContext(MyDbContext dbContext)
        {
            _myDbContext = dbContext;
        }
    }
}