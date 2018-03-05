﻿namespace NaftanRailway.BLL.Services.Specifications
{
    using NaftanRailway.BLL.Abstract.Specifications;

    public class AndSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> leftSpecification;

        private readonly ISpecification<T> rightSpecification;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.leftSpecification = left;
            this.rightSpecification = right;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return this.leftSpecification.IsSatisfiedBy(o)
                   && this.rightSpecification.IsSatisfiedBy(o);
        }
    }
}