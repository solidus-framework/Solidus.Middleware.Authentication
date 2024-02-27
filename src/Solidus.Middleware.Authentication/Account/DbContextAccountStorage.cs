using Microsoft.EntityFrameworkCore;

namespace Solidus.Middleware.Authentication.Account;

// TODO: Reference non-full EntityFrameworkCore.

/// <summary>
/// An account storage based on Identity Framework Core.
/// </summary>
public class DbContextAccountStorage<TDbContext, TAccount> : IAccountStorage, IAccountProvider<TAccount>
    where TDbContext : DbContext
    where TAccount : class
{
    private readonly TDbContext _dbContext;
    private readonly Lazy<DbSet<TAccount>> _dbSet;
    private readonly DbContextAccountAdapter<TAccount> _accountAdapter;

    /// <summary>
    /// Initializes a new instance of <see cref="EntityFrameworkCoreAccountStorage"/>.
    /// </summary>
    public DbContextAccountStorage(
        TDbContext dbContext,
        Func<TDbContext, DbSet<TAccount>> dbSetSelector,
        DbContextAccountAdapter<TAccount> accountAdapter)
    {
        _dbContext = dbContext;
        _dbSet = new(() => dbSetSelector(dbContext));
        _accountAdapter = accountAdapter;
    }

    /// <inheritdoc/>
    public async Task<string> CreateAccountAsync(
        string name,
        string passwordHash,
        IDictionary<string, string>? metadata,
        CancellationToken cancellationToken)
    {
        var account = _accountAdapter.CreateEntity();
        _accountAdapter.SetName(account, name);
        _accountAdapter.SetPasswordHash(account, passwordHash);

        try
        {
            _accountAdapter.SetMetadata(account, metadata);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid metadata format.", nameof(metadata), ex);
        }

        await _dbSet.Value.AddAsync(account, cancellationToken);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new ArgumentException("Name is already taken.", nameof(name), ex);
        }

        return _accountAdapter.GetId(account);
    }

    /// <inheritdoc/>
    public async Task<AccountData?> GetAccountDataByNameAsync(string name, CancellationToken cancellationToken)
    {
        var notDeletedQuery = _accountAdapter.FilterByNotDeleted(_dbSet.Value);
        var account = await _accountAdapter.FilterByName(notDeletedQuery, name).FirstOrDefaultAsync(cancellationToken);
        if (account == null)
        {
            return null;
        }

        var id = _accountAdapter.GetId(account);
        var passwordHash = _accountAdapter.GetPasswordHash(account);

        return new()
        {
            Id = id,
            Name = name,
            PasswordHash = passwordHash,
        };
    }

    /// <inheritdoc/>
    public async Task SetPasswordHashAsync(string accountId, string passwordHash, CancellationToken cancellationToken)
    {
        var account = await GetRequiredAccountAsync(accountId, cancellationToken);
        _accountAdapter.SetPasswordHash(account, passwordHash);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, string>?> GetMetadataAsync(string accountId, CancellationToken cancellationToken)
    {
        var account = await GetRequiredAccountAsync(accountId, cancellationToken);
        return _accountAdapter.GetMetadata(account);
    }

    /// <inheritdoc/>
    public async Task SetMetadataAsync(string accountId, IDictionary<string, string> metadata, CancellationToken cancellationToken)
    {
        var account = await GetRequiredAccountAsync(accountId, cancellationToken);
        _accountAdapter.SetMetadata(account, metadata);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAccountAsync(string accountId, CancellationToken cancellationToken)
    {
        var account = await GetRequiredAccountAsync(accountId, cancellationToken);
        _accountAdapter.SetIsDeleted(account, true);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RestoreAccountAsync(string accountId, CancellationToken cancellationToken)
    {
        var account = await GetRequiredAccountAsync(accountId, cancellationToken);
        _accountAdapter.SetIsDeleted(account, false);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TAccount?> GetAccountAsync(string accountId, CancellationToken cancellationToken)
    {
        var notDeletedQuery = _accountAdapter.FilterByNotDeleted(_dbSet.Value);
        return _accountAdapter.FilterById(notDeletedQuery, accountId).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<TAccount> GetRequiredAccountAsync(string accountId, CancellationToken cancellationToken)
    {
        var account = await GetAccountAsync(accountId, cancellationToken)
            ?? throw new ArgumentException($"Account with id '{accountId}' is not found.", nameof(accountId));

        return account;
    }
}
