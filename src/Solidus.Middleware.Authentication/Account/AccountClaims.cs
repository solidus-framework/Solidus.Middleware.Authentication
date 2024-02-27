using System.Security.Claims;

namespace Solidus.Middleware.Authentication.Account;

/// <summary>
/// An account claims.
/// </summary>
public class AccountClaims
{
    /// <summary>
    /// An account name claim.
    /// </summary>
    /// <remarks>
    /// Claim to be added to <see cref="ClaimsPrincipal"/>.
    /// Type specified by <see cref="SolidusAuthenticationOptions.NameClaimType"/>.
    /// </remarks>
    public required string Name { get; set; }

    /// <summary>
    /// An account role claim value <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <remarks>
    /// Claim to be added to <see cref="ClaimsPrincipal"/>.
    /// Type specified by <see cref="SolidusAuthenticationOptions.RoleClaimType"/>.
    /// </remarks>
    public required string Role { get; set; }

    /// <summary>
    /// Additional account claims to be added to <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <remarks>
    /// Claims to be added to <see cref="ClaimsPrincipal"/>.
    /// Should not contain claims with type specified by
    /// <see cref="SolidusAuthenticationOptions.NameClaimType"/> or
    /// <see cref="SolidusAuthenticationOptions.RoleClaimType"/>.
    /// </remarks>
    public required IEnumerable<Claim> AdditionalClaims { get; set; }
}
