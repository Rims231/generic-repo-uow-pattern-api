using generic_repo_uow_pattern_api.Data;
using NPOI.SS.Formula.Functions;

namespace generic_repo_uow_pattern_api.Repository
{




    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);


        void SetDbContext(MyDbContext dbContext);
    }
}
