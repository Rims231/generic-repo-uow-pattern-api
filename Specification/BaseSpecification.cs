using generic_repo_uow_pattern_api.Repository;
using System.Linq.Expressions;

namespace generic_repo_uow_pattern_api.Specification
{
    public abstract class BaseSpecification<T> : ISpecification<T> where T : class
    {


        public Expression<Func<T, bool>>? Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDesc { get; private set; }
        public int? Take { get; private set; }
        public int? Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }



        protected void AddCriteria(Expression<Func<T, bool>> criteria)
            => Criteria = criteria;

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
            => OrderBy = orderByExpression;

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
            => OrderByDesc = orderByDescExpression;

        protected void ApplyPaging(int pageNumber, int pageSize)
        {
            Skip = (pageNumber - 1) * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
        }
    }
}