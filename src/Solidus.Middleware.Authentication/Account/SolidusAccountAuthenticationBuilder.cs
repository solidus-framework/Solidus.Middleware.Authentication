using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An Solidus Account Authentication builder.
/// </summary>
/// <typeparam name="TAccount">The account entity type.</typeparam>
public class SolidusAccountAuthenticationBuilder<TAccount>
    where TAccount : class
{
    private readonly DependencyFactory<IPasswordHasher<DefaultAccountPasswordHasher>> _passwordHasherDep = new();
    private readonly DependencyFactory<IAccountClaimsFactory> _accountClaimsFactoryDep = new();
    private readonly DependencyFactory<IAccountPasswordHasher> _accountPasswordHasherDep = new();
    private readonly DependencyFactory<IAccountProvider<TAccount>> _accountProviderDep = new();
    private readonly DependencyFactory<IAccountService> _accountServiceDep = new();
    private readonly DependencyFactory<IAccountStorage> _accountStorageDep = new(
        "Account storage is not configured for Solidus Authentication accounts.");

    /// <summary>
    /// The services to be registered.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="SolidusAccountAuthenticationBuilder<TAccount>"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public SolidusAccountAuthenticationBuilder(IServiceCollection services)
    {
        Services = services;

        _passwordHasherDep.SetDefaultFactory<PasswordHasher<DefaultAccountPasswordHasher>>();
        _accountClaimsFactoryDep.SetDefaultFactory<DefaultAccountClaimsFactory>();
        _accountPasswordHasherDep.SetDefaultFactory<DefaultAccountPasswordHasher>();
        _accountServiceDep.SetDefaultFactory<DefaultAccountService>();

        services.TryAddScoped(_passwordHasherDep.CreateService);
        services.TryAddScoped(_accountClaimsFactoryDep.CreateService);
        services.TryAddScoped(_accountPasswordHasherDep.CreateService);
        services.TryAddScoped(_accountProviderDep.CreateService);
        services.TryAddScoped(_accountServiceDep.CreateService);
        services.TryAddScoped(_accountStorageDep.CreateService);
    }

    /// <summary>
    /// Registers Entity Framework Core DbContext as <see cref="IAccountStorage"/> and <see cref="IAccountProvider{TAccount}"/>
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    /// <typeparam name="TAccountAdapter">The account entity adapter.</typeparam>
    /// <param name="dbSetSelector">The account db set selector.</param>
    /// <returns>This builder instance.</returns>
    public SolidusAccountAuthenticationBuilder<TAccount> AddEntityFrameworkCoreStorage<TDbContext, TAccountAdapter>(
        Func<TDbContext, DbSet<TAccount>> dbSetSelector)
        where TDbContext : DbContext
        where TAccountAdapter : DbContextAccountAdapter<TAccount>
    {
        Services.TryAddScoped<DbContextAccountAdapter<TAccount>, TAccountAdapter>();

        _accountStorageDep.Factory = provider =>
        {
            var dbContext = provider.GetRequiredService<TDbContext>();
            var accountAdapter = provider.GetRequiredService<DbContextAccountAdapter<TAccount>>();
            return new DbContextAccountStorage<TDbContext, TAccount>(
                dbContext: dbContext,
                dbSetSelector: dbSetSelector,
                accountAdapter: accountAdapter
            );
        };

        _accountProviderDep.Factory = provider =>
        {
            var accountStorage = provider.GetRequiredService<IAccountStorage>();
            if (accountStorage is not DbContextAccountStorage<TDbContext, TAccount> dbContextAccountStorage)
            {
                throw new InvalidOperationException($"Unable to resolve '{typeof(IAccountProvider<TAccount>).FullName}'. "
                    + $"'{typeof(IAccountStorage).FullName}' service registration was overridden "
                    + $"by '{accountStorage.GetType().FullName}'.");
            }

            return dbContextAccountStorage;
        };

        return this;
    }
}
