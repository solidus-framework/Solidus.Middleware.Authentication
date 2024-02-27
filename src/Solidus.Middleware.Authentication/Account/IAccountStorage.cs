namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account storage.
/// </summary>
public interface IAccountStorage
{
    /// <summary>
    /// Creates an account.
    /// </summary>
    /// <param name="name">The account name.</param>
    /// <param name="passwordHash">The account password hash.</param>
    /// <param name="metadata">The key value collection of account metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The account id.</returns>
    /// <exception cref="ArgumentException">
    /// When account with specified <paramref name="name"/> is already exists in the storage.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// When <paramref name="metadata"/> contains invalid values or doesn't have required values.
    /// </exception>
    Task<string> CreateAccountAsync(
        string name,
        string passwordHash,
        IDictionary<string, string>? metadata,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets account id and password hash.
    /// </summary>
    /// <param name="name">The account name.</param>
    /// <param name="passwordHash">The account password hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The account id or null.</returns>
    Task<AccountData?> GetAccountDataByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Sets account password hash.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="passwordHash">The account password hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">
    /// When account with specified <paramref name="accountId"/> is not exist.
    /// </exception>
    Task SetPasswordHashAsync(string accountId, string passwordHash, CancellationToken cancellationToken);

    /// <summary>
    /// Gets account metadata.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The key value collection of account metadata.</returns>
    /// <exception cref="ArgumentException">
    /// When account with specified <paramref name="accountId"/> is not exist.
    /// </exception>
    Task<IDictionary<string, string>?> GetMetadataAsync(string accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Sets account metadata.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="metadata">The key value collection of account metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">
    /// When account with specified <paramref name="accountId"/> is not exist.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// When <paramref name="metadata"/> contains invalid values or doesn't have required values.
    /// </exception>
    Task SetMetadataAsync(string accountId, IDictionary<string, string> metadata, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes account with specified id.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">
    /// When account with specified <paramref name="accountId"/> is not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// When account is already deleted.
    /// </exception>
    Task DeleteAccountAsync(string accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Restores deleted account with specified id.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">
    /// When account with specified <paramref name="accountId"/> is not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// When account restoration procedure is not possible due to account contacts collision with existing active accounts.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// When account restoration procedure is not supported.
    /// </exception>
    Task RestoreAccountAsync(string accountId, CancellationToken cancellationToken);
}
