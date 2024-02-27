using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Solidus.Middleware.Authentication;

/// <summary>
/// An Solidus Authentication module builder.
/// </summary>
public class SolidusAuthenticationBuilder
{
    private readonly DependencyFactory<SolidusAuthenticationOptions> _authenticationOptionsDep = new();
    private readonly DependencyFactory<IAuthenticationService> _authenticationServiceDep = new();
    private Action<CookieBuilder>? _configureCookie;

    /// <summary>
    /// The services to be registered.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// The authentication options.
    /// </summary>
    public SolidusAuthenticationOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="SolidusAuthenticationBuilder"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public SolidusAuthenticationBuilder(IServiceCollection services)
    {
        Services = services;
        Options = new SolidusAuthenticationOptions();

        services.AddHttpContextAccessor();
        _authenticationOptionsDep.Factory = provider => Options;
        _authenticationServiceDep.SetDefaultFactory<SolidusAuthenticationService>();

        services.TryAddScoped(_authenticationOptionsDep.CreateService);
        services.TryAddScoped(_authenticationServiceDep.CreateService);
    }

    /// <summary>
    /// Setups authentication cookie configuration.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>This builder instance.</returns>
    public SolidusAuthenticationBuilder ConfigureCookie(Action<CookieBuilder> configure)
    {
        _configureCookie = configure;

        return this;
    }

    internal void ApplyCookieConfiguration(CookieBuilder builder)
    {
        builder.Name = "auth";
        builder.HttpOnly = true;
        builder.SecurePolicy = CookieSecurePolicy.None;
        builder.SameSite = SameSiteMode.Lax;
        builder.Expiration = null;
        builder.MaxAge = TimeSpan.MaxValue;

        _configureCookie?.Invoke(builder);
    }
}
