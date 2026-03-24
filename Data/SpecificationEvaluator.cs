using generic_repo_uow_pattern_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace generic_repo_uow_pattern_api.Specification
{
    public static class SpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// Applies all specification clauses (filter, includes, ordering, paging)
        /// to the supplied base query and returns the composed IQueryable.
        /// </summary>
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;

            // 1. WHERE
            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            // 2. INCLUDE (eager loading)
            query = spec.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            // 3. ORDER BY
            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDesc is not null)
                query = query.OrderByDescending(spec.OrderByDesc);

            // 4. PAGING
            if (spec.IsPagingEnabled && spec.Skip.HasValue && spec.Take.HasValue)
                query = query.Skip(spec.Skip.Value).Take(spec.Take.Value);

            return query;
        }
    }
}