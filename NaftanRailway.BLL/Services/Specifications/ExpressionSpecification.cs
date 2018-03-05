namespace NaftanRailway.BLL.Services.Specifications
{
    using System;

    public class ExpressionSpecification<T> : CompositeSpecification<T>
    {
        private readonly Func<T, bool> expression;

        public ExpressionSpecification(Func<T, bool> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                this.expression = expression;
            }
        }

        public override bool IsSatisfiedBy(T entity)
        {
            return this.expression(entity);
        }
    }
}