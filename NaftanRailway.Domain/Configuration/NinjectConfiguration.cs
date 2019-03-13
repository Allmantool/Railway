namespace Railway.Domain.Configuration
{
    using Ninject;

    public static class NinjectConfiguration
    {
        private static IKernel ApplyDomainContainerConfiguration(IKernel container)
        {
            return container;
        }
    }
}
