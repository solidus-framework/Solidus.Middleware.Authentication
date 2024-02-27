using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Solidus.Middleware.Authentication;

/// <summary>
/// The implementation of an authentication management service.
/// </summary>
public class SolidusAuthenticationService : IAuthenticationService
{
    private readonly SolidusAuthenticationOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes an instance of <see cref="SolidusAuthenticationService"/>.
    /// </summary>
    public SolidusAuthenticationService(SolidusAuthenticationOptions options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public async Task SignInAsync(
        string authenticationType,
        string name,
        string role,
        IEnumerable<Claim> additionalClaims,
        AuthenticationProperties? properties = null)
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException($"Unable to resolve HTTP context.");
        }
        if (additionalClaims.Any(c => c.Type == _options.NameClaimType))
        {
            throw new ArgumentException(
                $"Additional claims cannot contain '{_options.NameClaimType}' claim type that is reserved for {nameof(name)}.",
                nameof(additionalClaims));
        }
        if (additionalClaims.Any(c => c.Type == _options.RoleClaimType))
        {
            throw new ArgumentException(
                $"Additional claims cannot contain '{_options.RoleClaimType}' claim type that is reserved for {nameof(role)}.",
                nameof(additionalClaims));
        }

        var nameClaim = new Claim(
            type: _options.NameClaimType,
            value: name,
            valueType: null,
            issuer: _options.ClaimIssuer
        );
        var roleClaim = new Claim(
            type: _options.RoleClaimType,
            value: role,
            valueType: null,
            issuer: _options.ClaimIssuer
        );

        List<Claim> claims = [nameClaim, roleClaim];
        claims.AddRange(additionalClaims);

        var identity = new ClaimsIdentity(
            claims: claims,
            authenticationType: authenticationType,
            nameType: _options.NameClaimType,
            roleType: _options.RoleClaimType
        );
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext.SignInAsync(
            scheme: _options.AuthenticationScheme,
            principal: principal,
            properties: properties
        );
    }

    /// <inheritdoc/>
    public async Task SignOutAsync(AuthenticationProperties? properties = null)
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException("Unable to resolve HTTP context.");
        }

        await _httpContextAccessor.HttpContext.SignOutAsync(
            scheme: _options.AuthenticationScheme,
            properties: properties
        );
    }

    /// <inheritdoc/>
    public Task<AuthenticateResult> AuthenticateAsync()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException("Unable to resolve HTTP context.");
        }

        return _httpContextAccessor.HttpContext.AuthenticateAsync(scheme: _options.AuthenticationScheme);
    }
}
