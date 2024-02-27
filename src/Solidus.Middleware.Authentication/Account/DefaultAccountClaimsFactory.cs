namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// The default implementation of <see cref="IAccountClaimsFactory"/>.
/// </summary>
public class DefaultAccountClaimsFactory : IAccountClaimsFactory
{
    public const string DefaultAccountRole = "account";

    /// <inheritdoc>
    public Task<AccountClaims> CreateAccountClaimsAsync(string accountId, string authenticationType, CancellationToken cancellationToken)
    {
        var claims = new AccountClaims
        {
            Name = accountId,
            Role = DefaultAccountRole,
            AdditionalClaims = [],
        };

        return Task.FromResult(claims);
    }
}
