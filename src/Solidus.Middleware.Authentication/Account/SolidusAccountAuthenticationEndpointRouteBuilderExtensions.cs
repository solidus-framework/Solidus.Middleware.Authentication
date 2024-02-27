namespace Solidus.Middleware.Authentication.Account;

public static class SolidusAccountAuthenticationEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps Solidus Authentication account Sign Up action.
    /// <list type="bullet">
    /// <item><description>Sign Up Route: /account/sign-up</description></item>
    /// </list>
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <returns>The route group builder.</returns>
    public static RouteGroupBuilder MapSolidusAccountSignUpAction(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(AccountController.ControllerRoute);

        group.MapAreaControllerRoute(
            name: "Solidus.Middleware.Authentication.Account.AccountController.SignUp",
            areaName: ControllerHelpers.GetControllerArea<AccountController>(),
            pattern: "{action}",
            defaults: new
            {
                controller = ControllerHelpers.GetControllerRouteName<AccountController>()
            },
            constraints: new
            {
                action = ControllerHelpers.GetActionRouteConstraints<AccountController>(c => c.AccountSignUp)
            }
        );

        return group;
    }

    /// <summary>
    /// Maps Solidus Authentication account Sign In action.
    /// <list type="bullet">
    /// <item><description>Sign In Route: /account/sign-in</description></item>
    /// </list>
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <returns>The route group builder.</returns>
    public static RouteGroupBuilder MapSolidusAccountSignInAction(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(AccountController.ControllerRoute);

        group.MapAreaControllerRoute(
            name: "Solidus.Middleware.Authentication.Account.AccountController.SignIn",
            areaName: ControllerHelpers.GetControllerArea<AccountController>(),
            pattern: "{action}",
            defaults: new
            {
                controller = ControllerHelpers.GetControllerRouteName<AccountController>()
            },
            constraints: new
            {
                action = ControllerHelpers.GetActionRouteConstraints<AccountController>(c => c.AccountSignIn)
            }
        );

        return group;
    }
}
