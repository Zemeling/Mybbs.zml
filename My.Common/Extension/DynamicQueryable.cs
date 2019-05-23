using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Extension
{
    public static class DynamicQueryable
    {
        private static Dictionary<string, Expression> _cache = new Dictionary<string, Expression>();

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)((IQueryable)source).Where(predicate, values);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            LambdaExpression lambda = My.Common.Extension.DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, values);
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where", new Type[1]
            {
            source.ElementType
            }, source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            LambdaExpression lambda = My.Common.Extension.DynamicExpression.ParseLambda(source.ElementType, null, selector, values);
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select", new Type[2]
            {
            source.ElementType,
            lambda.Body.Type
            }, source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)((IQueryable)source).OrderBy(ordering, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (ordering == null)
            {
                throw new ArgumentNullException("ordering");
            }
            Type type = source.ElementType;
            ParameterExpression[] parameters = new ParameterExpression[1]
            {
            Expression.Parameter(type, "")
            };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, values);
            IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
            string methodAsc = "OrderBy";
            string methodDesc = "OrderByDescending";
            Expression queryExpr = source.Expression;
            foreach (DynamicOrdering item in orderings)
            {
                queryExpr = Expression.Call(typeof(Queryable), item.Ascending ? methodAsc : methodDesc, new Type[2]
                {
                source.ElementType,
                item.Selector.Type
                }, queryExpr, Expression.Quote(Expression.Lambda(item.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        public static IQueryable<T> SingleOrderBy<T>(this IQueryable<T> source, string sortProperty, bool ascending = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (string.IsNullOrEmpty(sortProperty))
            {
                throw new ArgumentNullException("sortProperty");
            }
            string ordering = ascending ? (sortProperty + " asc") : (sortProperty + " desc");
            return source.OrderBy(ordering);
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Take", new Type[1]
            {
            source.ElementType
            }, source.Expression, Expression.Constant(count)));
        }

        public static IQueryable Skip(this IQueryable source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Skip", new Type[1]
            {
            source.ElementType
            }, source.Expression, Expression.Constant(count)));
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TValue>(this IQueryable<TSource> source, Expression<Func<TSource, TValue>> selector, bool isDesc)
        {
            return isDesc ? source.OrderByDescending(selector) : source.OrderBy(selector);
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }
            LambdaExpression keyLambda = My.Common.Extension.DynamicExpression.ParseLambda(source.ElementType, null, keySelector, values);
            LambdaExpression elementLambda = My.Common.Extension.DynamicExpression.ParseLambda(source.ElementType, null, elementSelector, values);
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "GroupBy", new Type[3]
            {
            source.ElementType,
            keyLambda.Body.Type,
            elementLambda.Body.Type
            }, source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
        }

        public static bool Any(this IQueryable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return (bool)source.Provider.Execute(Expression.Call(typeof(Queryable), "Any", new Type[1]
            {
            source.ElementType
            }, source.Expression));
        }

        public static int Count(this IQueryable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return (int)source.Provider.Execute(Expression.Call(typeof(Queryable), "Count", new Type[1]
            {
            source.ElementType
            }, source.Expression));
        }

        private static void SetLambdaExpression(string cacheKey, Expression keySelector)
        {
            if (!DynamicQueryable._cache.ContainsKey(cacheKey))
            {
                DynamicQueryable._cache[cacheKey] = keySelector;
            }
        }

        private static Expression GetLambdaExpression(string cacheKey)
        {
            return DynamicQueryable._cache.ContainsKey(cacheKey) ? DynamicQueryable._cache[cacheKey] : null;
        }
    }
}
