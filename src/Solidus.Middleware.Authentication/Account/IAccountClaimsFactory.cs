namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account claims factory.
/// </summary>
public interface IAccountClaimsFactory
{
    /// <summary>
    /// Creates claims for specified account data.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="authenticationType">The type of an authentication used to sign in.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An account claims.</returns>
    Task<AccountClaims> CreateAccountClaimsAsync(string accountId, string authenticationType, CancellationToken cancellationToken);
}
