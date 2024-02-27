using Microsoft.AspNetCore.Authentication;

namespace Solidus.Middleware.Authentication;

public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds Solidus Authentication module to <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="builder">The authentication builder instance.</param>
    /// <returns>The authentication builder instance.</returns>
    public static AuthenticationBuilder AddSolidus(this AuthenticationBuilder builder)
    {
        return AddSolidus(builder, SolidusAuthenticationOptions.DefaultAuthenticationScheme);
    }

    /// <summary>
    /// Adds Solidus Authentication module to <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="builder">The authentication builder instance.</param>
    /// <param name="displayName">A display name for the authentication handler.</param>
    /// <returns>The authentication builder instance.</returns>
    public static AuthenticationBuilder AddSolidus(this AuthenticationBuilder builder, string authenticationScheme)
    {
        return AddSolidus(builder, authenticationScheme, c => {});
    }

    /// <summary>
    /// Adds Solidus Authentication module to <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="builder">The authentication builder instance.</param>
    /// <param name="configure">A delegate to configure authentication.</param>
    /// <returns>The authentication builder instance.</returns>
    public static AuthenticationBuilder AddSolidus(this AuthenticationBuilder builder, Action<SolidusAuthenticationBuilder> configure)
    {
        return AddSolidus(builder, SolidusAuthenticationOptions.DefaultAuthenticationScheme, configure);
    }

    /// <summary>
    /// Adds Solidus Authentication module to <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="builder">The authentication builder instance.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configure">A delegate to configure authentication.</param>
    /// <returns>The authentication builder instance.</returns>
    public static AuthenticationBuilder AddSolidus(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<SolidusAuthenticationBuilder> configure)
    {
        return AddSolidus(builder, authenticationScheme, null, configure);
    }

    /// <summary>
    /// Adds Solidus Authentication to <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="builder">The authentication builder instance.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">A display name for the authentication handler.</param>
    /// <param name="configure">A delegate to configure authentication.</param>
    /// <returns>The authentication builder instance.</returns>
    public static AuthenticationBuilder AddSolidus(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string? displayName,
        Action<SolidusAuthenticationBuilder> configure)
    {
        var authBuilder = new SolidusAuthenticationBuilder(builder.Services);
        configure(authBuilder);

        builder.AddCookie(authenticationScheme, displayName, config =>
        {
            authBuilder.ApplyCookieConfiguration(config.Cookie);

            config.ExpireTimeSpan = authBuilder.Options.SessionTimeSpan;
            config.SlidingExpiration = authBuilder.Options.SlidingSessionTimeSpan;
            config.LoginPath = authBuilder.Options.ChallengeUrl;
            config.ReturnUrlParameter = authBuilder.Options.ReturnUrlParameter;
        });

        return builder;
    }
}
