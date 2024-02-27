using Microsoft.AspNetCore.Identity;

namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// The default implementation of <see cref="IAccountPasswordHasher"/>.
/// </summary>
public class DefaultAccountPasswordHasher : IAccountPasswordHasher
{
    private readonly IPasswordHasher<DefaultAccountPasswordHasher> _passwordHasher;

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultAccountPasswordHasher"/>.
    /// </summary>
    /// <param name="passwordHasher">The password hasher.</param>
    public DefaultAccountPasswordHasher(IPasswordHasher<DefaultAccountPasswordHasher> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc/>
    public Task<string> HashAccountPasswordAsync(string password, CancellationToken cancellationToken)
    {
        var passwordHash = _passwordHasher.HashPassword(this, password);
        return Task.FromResult(passwordHash);
    }

    /// <inheritdoc/>
    public Task<AccountPasswordVerificationResult> VerifyHashedAccountPasswordAsync(
        string passwordHash,
        string password,
        CancellationToken cancellationToken)
    {
        var originalResult = _passwordHasher.VerifyHashedPassword(this, passwordHash, password);
        var result = originalResult switch
        {
            PasswordVerificationResult.Failed => AccountPasswordVerificationResult.Failed,
            PasswordVerificationResult.Success => AccountPasswordVerificationResult.Success,
            PasswordVerificationResult.SuccessRehashNeeded => AccountPasswordVerificationResult.SuccessRehashNeeded,
            _ => AccountPasswordVerificationResult.Failed,
        };

        return Task.FromResult(result);
    }
}
