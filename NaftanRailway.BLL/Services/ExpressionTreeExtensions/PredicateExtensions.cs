using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NaftanRailway.BLL.Services.ExpressionTreeExtensions {
    /// <summary>
    /// http://www.albahari.com/nutshell/predicatebuilder.aspx
    /// Expression have been represented as nodes of a data structure.
    /// The expression tree is code converted into structure data
    /// Expression trees were created in order to make the task of converting code such as a query expression into a string that can be passed to some other process and executed there
    /// if we need out of proccess (IQueryble) use Expression tree if not (IEnumerable) no use
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

        /// <summary>
        /// Get property name from some type instance by Linq (Someting wrong). Although use reflection with expression tree
        /// It's filtering field
        /// </summary>
        /// <typeparam name="T">generating type work's entity</typeparam>
        /// <param name="lambda">get property through lambda</param>
        /// <returns></returns>
        public static string GetPropName<T>(Expression<Func<T, object>> lambda) {
            //if we need extract addtitional nodyType (ExpressionType.Convert) => this for converting diffrent type to string
            var body = (lambda.Body as MemberExpression) ?? ((UnaryExpression)lambda.Body).Operand as MemberExpression;

            if (body == null) {
                throw new ArgumentException("PredicateExtensions.GetPropName error: body is null");
            }

            return body.Member.Name;
        }

        /// <summary>
        /// Because of the lambda expression, you get not only compile-time error checking, but also full IntelliSense support when typing a member name. 
        /// And if you rename a property, the compiler will find all the places where the property name is used. 
        /// Or you can rely on refactoring tools to rename all the instances of the property for you.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetName<T>(Expression<Func<T>> e) {
            var member = (MemberExpression)e.Body;
            return member.Member.Name;
        }


        /// <summary>
        ///Implement visitor pattern.
        ///In case we want add some functionality in class without changed it.
        ///My situation is: I've expression tree with concrete class and predicate lambda expression (e.g Expreesion<Func<ClassA, bool>> predicate = x => x.prop == someValue)
        ///when i want save body predicate (x => x.prop == someValue), but change parameter type (ClassA to ClassB with same naming of propeties) I need implement visitor pattern
        /// </summary>
        /// <param name="ConvertToType">typeOf(Destination Class)</param>
        /// <param name=" expression">source expression</param>
        public static Expression<Func<OutT, bool>> ConvertTypeExpression<inT, OutT>(Expression expression) where OutT : class {

            var param = Expression.Parameter(typeof(OutT), "x");

            var result = new CustomExpVisitor<OutT>(param).Visit(expression);

            Expression<Func<OutT, bool>> lambda = Expression.Lambda<Func<OutT, bool>>(result, new[] { param });

            return lambda;
        }

        //build-in class (LINQ). Start in c# 4.0
        private class CustomExpVisitor<T> : ExpressionVisitor {
            ParameterExpression _param;

            public CustomExpVisitor(ParameterExpression param) {
                _param = param;
            }

            protected override Expression VisitParameter(ParameterExpression node) {
                return _param;
            }

            protected override Expression VisitMember(MemberExpression node) {
                if (node.Member.MemberType == MemberTypes.Property) {
                    MemberExpression memberExpression = null;

                    var memberName = node.Member.Name;
                    var otherMember = typeof(T).GetProperty(memberName);

                    memberExpression = Expression.Property(Visit(node.Expression), otherMember);

                    return memberExpression;
                } else {
                    return base.VisitMember(node);
                }
            }
        }
    }
}