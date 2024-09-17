namespace Rokalo.Infrastructure.Db.Users.Extensions
{
    using System.Linq.Expressions;
    using System.Linq;
    using System;

    internal static class QueryableExtensions
    {
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }

            return source;
        }
    }
}
