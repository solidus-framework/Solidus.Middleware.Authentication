namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account entity operations adapter.
/// </summary>
/// <typeparam name="TAccount">The account entity type.</typeparam>
public abstract class DbContextAccountAdapter<TAccount>
{
    /// <summary>
    /// Creates an account entity instance.
    /// </summary>
    /// <returns>An account entity instance.</returns>
    public abstract TAccount CreateEntity();

    /// <summary>
    /// Gets an entity id.
    /// </summary>
    /// <param name="account">The account entity.</param>
    /// <returns>The account id.</returns>
    public abstract string GetId(TAccount account);

    /// <summary>
    /// Gets an entity name.
    /// </summary>
    /// <param name="account">The account entity.</param>
    /// <returns>The account name.</returns>
    public abstract string GetName(TAccount account);

    /// <summary>
    /// Sets an entity name.
    /// </summary>
    /// <param name="account">The account entity.</param>
    /// <param name="name">The account name.</param>
    public abstract void SetName(TAccount account, string name);

    /// <summary>
    /// Gets an entity password hash.
    /// </summary>
    /// <param name="account">The account name.</param>
    /// <returns>The account name.</returns>
    public abstract string GetPasswordHash(TAccount account);

    /// <summary>
    /// Sets an entity password hash.
    /// </summary>
    /// <param name="account">The account entity.</param>
    /// <param name="passwordHash">The account password hash.</param>
    public abstract void SetPasswordHash(TAccount account, string passwordHash);

    /// <summary>
    /// Gets an entity metadata.
    /// </summary>
    /// <param name="account">The account name.</param>
    /// <returns>The account metadata.</returns>
    public abstract IDictionary<string, string>? GetMetadata(TAccount account);

    /// <summary>
    /// Sets an entity metadata.
    /// </summary>
    /// <param name="account">The account entity.</param>
    /// <param name="metadata">The account metadata.</param>
    /// <exception cref="ArgumentException">
    /// When metadata has invalid format.
    /// </exception>
    public abstract void SetMetadata(TAccount account, IDictionary<string, string>? metadata);

    /// <summary>
    /// Gets an entity is deleted state.
    /// </summary>
    /// <param name="account">The account name.</param>
    /// <returns>The account is deleted state.</returns>
    public abstract bool GetIsDeleted(TAccount account);

    /// <summary>
    /// Sets an entity is deleted state.
    /// </summary>
    /// <param name="account">The account entity.</param>
    /// <param name="isDeleted">The account is deleted state.</param>
    public abstract void SetIsDeleted(TAccount account, bool isDeleted);

    /// <summary>
    /// Makes query that is filtered by not deleted state.
    /// </summary>
    /// <param name="query">The accounts query.</param>
    /// <returns>The filtered accounts query.</returns>
    public abstract IQueryable<TAccount> FilterByNotDeleted(IQueryable<TAccount> query);

    /// <summary>
    /// Makes query that is filtered by account id.
    /// </summary>
    /// <param name="query">The accounts query.</param>
    /// <returns>The filtered accounts query.</returns>
    public abstract IQueryable<TAccount> FilterById(IQueryable<TAccount> query, string id);

    /// <summary>
    /// Makes query that is filtered by account name.
    /// </summary>
    /// <param name="query">The accounts query.</param>
    /// <returns>The filtered accounts query.</returns>
    public abstract IQueryable<TAccount> FilterByName(IQueryable<TAccount> query, string name);
}
