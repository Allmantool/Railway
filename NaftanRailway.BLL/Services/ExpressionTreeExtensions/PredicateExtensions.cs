using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NaftanRailway.BLL.Services.ExpressionTreeExtensions {
    /// <summary>
    /// http://www.albahari.com/nutshell/predicatebuilder.aspx
    /// </summary>
    public static class PredicateExtensions {
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

        /// <summary>
        /// Main containt method (avoid err: в данном контексте можно использовать только типы примитивы)
        /// Расширение позволяет  работать динамически отсылая на сервер TSQL запрос (IN (arg1,arg2.. arg-n)
        /// Linq to Entity don't support Invoke - the method you're actually calling when you call a Func or Action
        /// has no consistent way to transform it into a sql statement
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="colItems"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> InnerContainsPredicate<TEntity, T>(string propertyName, IEnumerable<T> colItems) where TEntity : class {
            ParameterExpression entity = Expression.Parameter(typeof(TEntity), "srcObj"); //class people
            MemberExpression member = Expression.Property(entity, propertyName); // property people.Age

            var someValue = Expression.Constant(colItems);
            var body = Expression.Call(typeof(Enumerable), "Contains", new[] { member.Type }, someValue, member);

            return Expression.Lambda<Func<TEntity, bool>>(body, entity);
        }

        /// <summary>
        /// Build lambda expression (in this particular way for groupBy operation), in another side expresssion is general => this's mean it's be suitable for many cases
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Expression<Func<T, string>> GroupPredicate<T>(string fieldName) {
            //A ParameterExpression denotes the input variable to the left hand side of the lambda (=>) operator in the lambda expression.
            //name "x" is equal x=>...
            var param = Expression.Parameter(typeof(T), "x");
            //A MemberExpression denotes property of type e.t x=>x.[fieldName]
            var property = Expression.Property(param, fieldName);
            //important to use the Expression.Convert
            //Expression conversion = Expression.Convert(property, typeof(string));

            //Reflection
            MethodInfo toStrMethod = typeof(object).GetMethod("ToString");

            //body of lambda expression (delegate that encapsulates a method with requer parameters)
            //Basically with expression three we build hard-wired lambda expression (etc. x=>x.Name )
            return Expression.Lambda<Func<T, string>>(Expression.Call(property, toStrMethod), param);
        }

        public static Expression<Func<TItem, bool>> PropertyEquals<TItem, TValue>(PropertyInfo property, TValue value) {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Equal(Expression.Property(param, property),
                Expression.Constant(value));
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        public static Expression<Func<TEntity, bool>> ContainsPredicate<TEntity, T>(T[] arr, string fieldname) where TEntity : class {
            ParameterExpression entity = Expression.Parameter(typeof(TEntity), "entity");
            MemberExpression member = Expression.Property(entity, fieldname);

            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "Contains");
            MethodInfo method = null;
            foreach (var m in containsMethods) {
                if (m.GetParameters().Count() == 2) {
                    method = m;
                    break;
                }
            }
            method = method.MakeGenericMethod(member.Type);
            var exprContains = Expression.Call(method, new Expression[] { Expression.Constant(arr), member });
            return Expression.Lambda<Func<TEntity, bool>>(exprContains, entity);
        }

        /// <summary>
        /// Replace for Convert function in Linq to SQL
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <returns></returns>
        public static Expression<Func<TInput, int>> ConvertInt32<TInput>(string fieldName, string propertyValue) {
            ParameterExpression param = Expression.Parameter(typeof(TInput), "type");
            MemberExpression expr = Expression.Property(param, fieldName);

            MethodInfo convertMethod = typeof(Convert).GetMethod("ToInt32");
            //MethodInfo splitMethod = typeof (string).GetMethod("Split");
            ConstantExpression someValue = Expression.Constant(propertyValue, typeof(string));

            var containsMethodExp = Expression.Call(expr, convertMethod, someValue);

            return Expression.Lambda<Func<TInput, int>>(containsMethodExp, param);
        }
    }
}