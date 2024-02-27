using Solidus.Middleware.Authentication.Account;

namespace Solidus.Middleware.Authentication;

public static class SolidusAuthenticationBuilderExtensions
{
    /// <summary>
    /// Registers account-related services for Solidus Authentication module.
    /// </summary>
    /// <typeparam name="TAccount">The account type.</typeparam>
    /// <param name="builder">The solidus authentication builder.</param>
    /// <returns>The solidus authentication builder.</returns>
    public static SolidusAccountAuthenticationBuilder<TAccount> AddAccounts<TAccount>(this SolidusAuthenticationBuilder builder)
        where TAccount : class
    {
        return new SolidusAccountAuthenticationBuilder<TAccount>(builder.Services);
    }
}
