namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// The default implementation of <see cref="IAccountService"/>.
/// </summary>
public class DefaultAccountService : IAccountService
{
    private readonly IAccountStorage _accountStorage;
    private readonly IAccountPasswordHasher _accountPasswordHasher;

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultAccountService"/>.
    /// </summary>
    public DefaultAccountService(
        IAccountStorage accountStorage,
        IAccountPasswordHasher accountPasswordHasher)
    {
        _accountStorage = accountStorage;
        _accountPasswordHasher = accountPasswordHasher;
    }

    /// <inheritdoc/>
    public async Task<string> CreateAccountAsync(
        string name,
        string password,
        IDictionary<string, string>? metadata,
        CancellationToken cancellationToken)
    {
        var passwordHash = await _accountPasswordHasher.HashAccountPasswordAsync(password, cancellationToken);
        return await _accountStorage.CreateAccountAsync(name, passwordHash, metadata, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<string?> AuthenticateAccountAsync(string name, string password, CancellationToken cancellationToken)
    {
        var account = await _accountStorage.GetAccountDataByNameAsync(name, cancellationToken);
        if (account == null)
        {
            return null;
        }

        var result = await _accountPasswordHasher.VerifyHashedAccountPasswordAsync(account.PasswordHash, password, cancellationToken);
        switch (result)
        {
            case AccountPasswordVerificationResult.Failed:
                return null;
            case AccountPasswordVerificationResult.Success:
                return account.Id;
            case AccountPasswordVerificationResult.SuccessRehashNeeded:
                var newPasswordHash = await _accountPasswordHasher.HashAccountPasswordAsync(password, cancellationToken);
                await _accountStorage.SetPasswordHashAsync(account.Id, newPasswordHash, cancellationToken);

                return account.Id;
            default:
                throw new NotImplementedException($"Unknown password verification result: '{result}'.");
        }
    }
}
