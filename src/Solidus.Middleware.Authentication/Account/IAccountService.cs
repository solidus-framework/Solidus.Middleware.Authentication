namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account management service.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Creates an account with hashed passwords.
    /// </summary>
    /// <param name="name">The account name.</param>
    /// <param name="password">The account password.</param>
    /// <param name="metadata">The key value collection of account metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identity id.</returns>
    /// <exception cref="ArgumentException">
    /// When account with specified <paramref name="name"/> is already exists in the storage.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// When <paramref name="metadata"/> contains invalid values or doesn't have required values.
    /// </exception>
    Task<string> CreateAccountAsync(
        string name,
        string password,
        IDictionary<string, string>? metadata,
        CancellationToken cancellationToken);

    /// <summary>
    /// Authenticates an account by specified credentials.
    /// </summary>
    /// <param name="name">The account name.</param>
    /// <param name="contact">The account password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The account id or null.</returns>
    Task<string?> AuthenticateAccountAsync(string name, string password, CancellationToken cancellationToken);
}
