using Microsoft.AspNetCore.Mvc;

namespace Solidus.Middleware.Authentication;

[Area("Solidus.Middleware.Authentication")]
public class SessionController : ControllerBase
{
    public const string ControllerRoute = "";
    public const string SessionStatusActionRoute = "status";
    public const string SessionSignOutActionRoute = "sign-out";

    private readonly IAuthenticationService _authenticationService;

    public SessionController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet]
    [ActionName(SessionStatusActionRoute)]
    public async Task<IActionResult> SessionStatus()
    {
        var authResult = await _authenticationService.AuthenticateAsync();
        if (!authResult.Succeeded)
        {
            return Unauthorized();
        }

        Dictionary<string, string> claims = [];
        foreach (var claim in User.Claims)
        {
            claims[claim.Type] = claim.Value;
        }

        var response = new SessionStatusResponseModel
        {
            Claims = claims,
        };

        return Ok(response);
    }

    [HttpPost]
    [ActionName(SessionSignOutActionRoute)]
    public async Task<IActionResult> SessionSignOut()
    {
        var authResult = await _authenticationService.AuthenticateAsync();
        if (!authResult.Succeeded)
        {
            return Unauthorized();
        }

        await _authenticationService.SignOutAsync();

        return Ok();
    }

    public class SessionStatusResponseModel
    {
        public required IDictionary<string, string> Claims { get; set; }
    }
}
