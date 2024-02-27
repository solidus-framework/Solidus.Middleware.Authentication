using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Solidus.Middleware.Authentication;

/// <summary>
/// An authentication management service.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Sign in a principal for the Solidus authentication scheme.
    /// </summary>
    /// <param name="authenticationType">The type of an authentication used to sign in.</param>
    /// <param name="name">The name claim value.</param>
    /// <param name="role">The role claim value.</param>
    /// <param name="additionalClaims">An additional claims.</param>
    /// <param name="properties">The authentication properties.</param>
    /// <exception cref="InvalidOperationException">
    /// When unable to access <see cref="HttpContext"/> with <see cref="IHttpContextAccessor"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// When <paramref name="additionalClaims"/> contains claims of type specified for <paramref name="name"/>
    /// and <paramref name="role"/>. Those claim types specified by <see cref="SolidusAuthenticationOptions"/>.
    /// </exception>
    Task SignInAsync(
        string authenticationType,
        string name,
        string role,
        IEnumerable<Claim> additionalClaims,
        AuthenticationProperties? properties = null);

    /// <summary>
    /// Sign out a principal for the Solidus authentication scheme.
    /// </summary>
    /// <param name="properties">The authentication properties.</param>
    /// <exception cref="InvalidOperationException">
    /// When unable to access <see cref="HttpContext"/> with <see cref="IHttpContextAccessor"/>.
    /// </exception>
    Task SignOutAsync(AuthenticationProperties? properties = null);

    /// <summary>
    /// Authenticate the current request.
    /// </summary>
    /// <returns>The authentication result.</returns>
    /// <exception cref="InvalidOperationException">
    /// When unable to access <see cref="HttpContext"/> with <see cref="IHttpContextAccessor"/>.
    /// </exception>
    Task<AuthenticateResult> AuthenticateAsync();
}
