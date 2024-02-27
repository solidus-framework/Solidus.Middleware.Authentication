namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account provider.
/// </summary>
/// <typeparam name="TAccount">The account type.</typeparam>
public interface IAccountProvider<TAccount>
{
    /// <summary>
    /// Gets an account by id.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The account.</returns>
    Task<TAccount?> GetAccountAsync(string accountId, CancellationToken cancellationToken);
}
