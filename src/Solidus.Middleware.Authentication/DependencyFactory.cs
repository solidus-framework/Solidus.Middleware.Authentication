namespace Solidus.Middleware.Authentication;

// TODO: Move to common project.

internal class DependencyFactory<TDependency>
{
    private readonly Lazy<Func<IServiceProvider, TDependency>> _lazyFactoryAccessor;
    private readonly string _factoryNotConfiguredError;

    public Func<IServiceProvider, TDependency>? Factory { get; set; }

    public DependencyFactory(string? factoryNotConfiguredError = null)
    {
        _lazyFactoryAccessor = new(GetFactory);
        _factoryNotConfiguredError = factoryNotConfiguredError ?? $"Factory for type '{typeof(TDependency).FullName}' is not registered.";
    }

    public void SetDefaultFactory<TImplementation>()
        where TImplementation : TDependency
    {
        Factory = provider => ActivatorUtilities.CreateInstance<TImplementation>(provider);
    }

    public TDependency CreateService(IServiceProvider provider) => _lazyFactoryAccessor.Value(provider);

    private Func<IServiceProvider, TDependency> GetFactory() => Factory
        ?? throw new InvalidOperationException(_factoryNotConfiguredError);
}
