namespace NaftanRailway.BLL.Services.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Abstract.Specifications;

    public class NotSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> specification;

        public NotSpecification(ISpecification<T> specification)
        {
            this.specification = specification;
        }

        public override bool IsSatisfiedBy(T entity)
        {
            return !this.specification.IsSatisfiedBy(entity);
        }

        public override Expression<Func<T, bool>> IsSatisfiedBy()
        {
            throw new NotImplementedException();
        }
    }
}