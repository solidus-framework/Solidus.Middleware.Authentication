namespace Solidus.Middleware.Authentication;

/// <summary>
/// An authentication options.
/// </summary>
public class SolidusAuthenticationOptions
{
    /// <summary>
    /// The default authentication scheme.
    /// </summary>
    public const string DefaultAuthenticationScheme = "solidus";

    private string _nameClaimType = "sub";
    private string _roleClaimType = "role";

    /// <summary>
    /// The authentication scheme.
    /// </summary>
    public string AuthenticationScheme { get; set; } = DefaultAuthenticationScheme;

    /// <summary>
    /// Amount of time session remains valid from the start.
    /// </summary>
    public TimeSpan SessionTimeSpan { get; set; } = TimeSpan.FromHours(8);

    /// <summary>
    /// If enabled session time will be extended when request made within a halfway expired session.
    /// </summary>
    public bool SlidingSessionTimeSpan { get; set; } = false;

    /// <summary>
    /// The redirect url for browser when request requires authentication.
    /// </summary>
    public string? ChallengeUrl { get; set; }

    /// <summary>
    /// The original url parameter name that will be included to <see cref="ChallengeUrl"/> on redirect.
    /// </summary>
    public string ReturnUrlParameter { get; set; } = "ReturnUrl";

    /// <summary>
    /// The claims issuer.
    /// </summary>
    public string? ClaimIssuer { get; set; } = null;

    /// <summary>
    /// The name claim type.
    /// </summary>
    /// <remarks>
    /// Cannot be null, empty or same as <see cref="RoleClaimType"/>.
    /// </remarks>
    public string NameClaimType
    {
        get => _nameClaimType;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Cannot be null or empty.", nameof(value));
            }
            if (value == _roleClaimType)
            {
                throw new ArgumentException($"Cannot be same as '{nameof(RoleClaimType)}'.", nameof(value));
            }

            _nameClaimType = value;
        }
    }

    /// <summary>
    /// The role claim type.
    /// </summary>
    /// <remarks>
    /// Cannot be null, empty or same as <see cref="NameClaimType"/>.
    /// </remarks>
    public string RoleClaimType
    {
        get => _roleClaimType;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Cannot be null or empty.", nameof(value));
            }
            if (value == _nameClaimType)
            {
                throw new ArgumentException($"Cannot be same as '{nameof(NameClaimType)}'.", nameof(value));
            }

            _roleClaimType = value;
        }
    }
}
