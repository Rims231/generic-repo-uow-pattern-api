using generic_repo_uow_pattern_api.Data;
using Microsoft.EntityFrameworkCore;


namespace generic_repo_uow_pattern_api.Repository
{


    public class Repository<T> : IRepository<T> where T : class
    {
        private  MyDbContext _myDbContext;
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

            if (entity == null)
                return null;

            _dbSet.Remove(entity);
            await _myDbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _myDbContext.Entry(entity).State = EntityState.Modified;

            await _myDbContext.SaveChangesAsync();

            return entity;
        }

        public void SetDbContext(MyDbContext dbContext)
        {
            _myDbContext= dbContext;
            
        }
    }


}

