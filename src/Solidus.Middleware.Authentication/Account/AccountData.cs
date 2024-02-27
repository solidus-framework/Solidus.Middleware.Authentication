namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account data.
/// </summary>
public class AccountData
{
    /// <summary>
    /// The account id.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The account name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The password hash.
    /// </summary>
    public required string PasswordHash { get; set; }
}
