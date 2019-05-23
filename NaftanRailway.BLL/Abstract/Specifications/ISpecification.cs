namespace NaftanRailway.BLL.Abstract.Specifications
{
    using System;
    using System.Linq.Expressions;

    public interface ISpecification<T>
    {
        ISpecification<T> And(ISpecification<T> specification);

        ISpecification<T> Not(ISpecification<T> specification);

        ISpecification<T> Or(ISpecification<T> specification);

        bool IsSatisfiedBy(T entity);

        Expression<Func<T, bool>> IsSatisfiedBy();
    }
}