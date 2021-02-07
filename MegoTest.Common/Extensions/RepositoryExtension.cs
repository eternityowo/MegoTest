using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MegoTest.Common.Extensions
{
    public static class RepositoryExtensions
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string sortField, bool descending)
        {
            var param = Expression.Parameter(typeof(T), "p");

            string[] sortFieldPath = sortField.Split('.');
            MemberExpression prop = Expression.Property(param, FirstLetterToUpper(sortFieldPath[0]));
            for (int i = 1; i < sortFieldPath.Length; i++)
            {
                prop = Expression.Property(prop, FirstLetterToUpper(sortFieldPath[i]));
            }

            var exp = Expression.Lambda(prop, param);
            var method = !descending ? "OrderBy" : "OrderByDescending";
            var types = new[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }

        private static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static IQueryable<T> GetPage<T>(this IQueryable<T> query, out int total, string orderBy, int skip, int take = 0,
            bool descOrder = true, IEnumerable<Expression<Func<T, bool>>> conditions = null)
        {
            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    query = query.Where(condition);
                }
            }

            if (skip == 0 && take == 0)
            {
                if (orderBy != null)
                {
                    query = query.OrderByField(orderBy, descOrder);
                }
                total = 0;
                try
                {
                    total = query.Count();
                }
                catch { }

                return query;
            }

            query = query.OrderByField(orderBy, descOrder);
            total = query.Count();
            query = query.Skip(skip);
            if (take != 0)
            {
                query = query.Take(take);
            }
            return query;
        }
    }
}
