using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NaftanRailway.Domain.ExpressionTreeExtensions {
    /// <summary>
    /// http://www.albahari.com/nutshell/predicatebuilder.aspx
    /// </summary>
    public static class EtExtensions {
        /// <summary>
        /// Task: greate expression tree and compile into equalent Linq to Sql/ Linq to Entity/ Sql request (contain)
        /// </summary>
        /// <param name="fieldName">Name of field</param>
        /// <param name="propertyValue">Value of field (single)</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> FilterByName<T>(string fieldName, string propertyValue) where T : class {
            ParameterExpression arg = Expression.Parameter(typeof(T), "typeEntity");
            MemberExpression expr = Expression.Property(arg, fieldName);

            MethodInfo containtMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            MethodInfo toStrMethod = typeof(object).GetMethod("ToString");

            ConstantExpression someValue = Expression.Constant(propertyValue, typeof(string));

            var containsMethodExp = Expression.Call(Expression.Call(expr, toStrMethod), containtMethod, someValue);

            return Expression.Lambda<Func<T, bool>>(containsMethodExp, arg);
        }

        public static Expression<Func<TEntity, bool>> ContainsPredicate<TEntity, T>(T[] arr, string fieldname) where TEntity : class {
            ParameterExpression entity = Expression.Parameter(typeof(TEntity), "entityType");
            MemberExpression member = Expression.Property(entity, fieldname);

            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) }).MakeGenericMethod(member.Type);

            var exprContains = Expression.Call(method, new Expression[] { Expression.Constant(arr), member });

            return Expression.Lambda<Func<TEntity, bool>>(exprContains, entity);
        }

        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel) {
            var param = Expression.Parameter(typeof(T), "p");
            var property = Expression.PropertyOrField(param, propertyName);
            var sort = Expression.Lambda(property, param);

            var call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName) {
            return OrderingHelper(source, propertyName, false, false);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool descending) {
            return OrderingHelper(source, propertyName, descending, false);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName) {
            return OrderingHelper(source, propertyName, false, true);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName, bool descending) {
            return OrderingHelper(source, propertyName, descending, true);
        }
    }
}
