using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Solidus.Middleware.Authentication.Account;

[Area("Solidus.Middleware.Authentication.Account")]
public class AccountController : ControllerBase
{
    public const string AuthenticationType = "credentials";

    public const string ControllerRoute = "account";
    public const string AccountSignUpActionRoute = "sign-up";
    public const string AccountSignInActionRoute = "sign-in";

    private readonly IAccountService _accountService;
    private readonly IAccountClaimsFactory _accountClaimsFactory;
    private readonly IAuthenticationService _authenticationService;

    public AccountController(
        IAccountService accountService,
        IAccountClaimsFactory accountClaimsFactory,
        IAuthenticationService authenticationService)
    {
        _accountService = accountService;
        _accountClaimsFactory = accountClaimsFactory;
        _authenticationService = authenticationService;
    }

    [HttpPost]
    [ActionName(AccountSignUpActionRoute)]
    public async Task<IActionResult> AccountSignUp([FromBody] AccountSignUpRequestModel model, CancellationToken cancellationToken)
    {
        string accountId;
        try
        {
            accountId = await _accountService.CreateAccountAsync(model.Name, model.Password, model.Metadata, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            if (ex.ParamName == "name")
            {
                return Conflict($"Name '{model.Name}' is already taken.");
            }
            if (ex.ParamName == "metadata")
            {
                return BadRequest("Invalid metadata values provided.");
            }

            throw new InvalidOperationException("Unable to create account due to unexpected error.", ex);
        }

        var claims = await _accountClaimsFactory.CreateAccountClaimsAsync(accountId, AuthenticationType, cancellationToken);
        var authProperties = GetAuthenticationProperties(model.RememberMe);
        await _authenticationService.SignInAsync(AuthenticationType, claims.Name, claims.Role, claims.AdditionalClaims, authProperties);

        return Ok();
    }

    [HttpPost]
    [ActionName(AccountSignInActionRoute)]
    public async Task<IActionResult> AccountSignIn([FromBody] AccountSignInRequestModel model, CancellationToken cancellationToken)
    {
        var accountId = await _accountService.AuthenticateAccountAsync(model.Name, model.Password, cancellationToken);
        if (accountId == null)
        {
            return Unauthorized();
        }

        var claims = await _accountClaimsFactory.CreateAccountClaimsAsync(accountId, AuthenticationType, cancellationToken);
        var authProperties = GetAuthenticationProperties(model.RememberMe);
        await _authenticationService.SignInAsync(AuthenticationType, claims.Name, claims.Role, claims.AdditionalClaims, authProperties);

        return Ok();
    }

    private static AuthenticationProperties GetAuthenticationProperties(bool isPersistent) => new()
    {
        IssuedUtc = DateTimeOffset.UtcNow,
        IsPersistent = isPersistent,
    };

    public class AccountSignUpRequestModel
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public IDictionary<string, string>? Metadata { get; set; }
        public required bool RememberMe { get; set; }
    }

    public class AccountSignInRequestModel
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required bool RememberMe { get; set; }
    }
}
