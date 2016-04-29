using System;
using System.Linq.Expressions;

namespace NaftanRailway.Domain.ExpressionTreeExtensions {
    public static class EtExtensions<T> where T : class {
        /// <summary>
        /// Task: greate expression tree and compile into equalent Linq to Sql/ Linq to Entity/ Sql request (contain)
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExtContains(string propertyName, string propertyValue) {
            var parameterExp = Expression.Parameter(typeof(T), "type");
            var propertyExp = Expression.Property(parameterExp, propertyName);
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(propertyValue, typeof(string));
            var containsMethodExp = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
        }
    }
}
