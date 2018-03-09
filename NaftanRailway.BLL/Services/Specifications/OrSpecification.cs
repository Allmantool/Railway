namespace NaftanRailway.BLL.Services.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Abstract.Specifications;

    public class OrSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> leftSpecification;

        private readonly ISpecification<T> rightSpecification;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.leftSpecification = left;
            this.rightSpecification = right;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return this.leftSpecification.IsSatisfiedBy(o)
                   || this.rightSpecification.IsSatisfiedBy(o);
        }

        public override Expression<Func<T, bool>> IsSatisfiedBy()
        {
            throw new NotImplementedException();
        }
    }
}