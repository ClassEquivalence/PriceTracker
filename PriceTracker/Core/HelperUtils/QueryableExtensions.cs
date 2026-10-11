using System.Linq.Expressions;

namespace PriceTracker.Core.HelperUtils
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> WhereNot<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var notBody = Expression.Not(predicate.Body);
            var invertedPredicate = Expression.Lambda<Func<TSource, bool>>(notBody, predicate.Parameters);
            return source.Where(invertedPredicate);
        }
    }
}
