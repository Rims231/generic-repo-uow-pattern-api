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

 
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

      
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
            => predicate is null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);

     
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

    
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(
            Expression<Func<T, TResult>> selector)
            => await _dbSet.Select(selector).ToListAsync();

     
        public async Task<IEnumerable<T>> FindWithOrderAsync<TKey>(
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, TKey>> orderBy,
            bool ascending = true)
        {
            var query = predicate is null ? _dbSet.AsQueryable() : _dbSet.Where(predicate);
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWithSpecAsync(ISpecification<T> spec)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .ToListAsync();

        public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .FirstOrDefaultAsync();

     
        public async Task<int> CountWithSpecAsync(ISpecification<T> spec)
            => await SpecificationEvaluator<T>
                .GetQuery(_dbSet.AsQueryable(), spec)
                .CountAsync();


        public void SetDbContext(MyDbContext dbContext)
        {
            _myDbContext = dbContext;
        }
    }
}