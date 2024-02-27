namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account password hasher.
/// </summary>
public interface IAccountPasswordHasher
{
    /// <summary>
    /// Makes a hash for the specified <paramref name="password"/>.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The password hash.</returns>
    Task<string> HashAccountPasswordAsync(string password, CancellationToken cancellationToken);

    /// <summary>
    /// Verifies <paramref name="password"/> against <paramref name="passwordHash"/>.
    /// </summary>
    /// <param name="passwordHash">The original password hash.</param>
    /// <param name="password">The password to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The password verification result..</returns>
    Task<AccountPasswordVerificationResult> VerifyHashedAccountPasswordAsync(
        string passwordHash,
        string password,
        CancellationToken cancellationToken);
}
