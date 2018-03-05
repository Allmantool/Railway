namespace NaftanRailway.BLL.Services.Specifications
{
    using NaftanRailway.BLL.Abstract.Specifications;

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
    }
}