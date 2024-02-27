namespace Solidus.Middleware.Authentication;

public static class SolidusAuthenticationEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps Solidus Authentication Status action.
    /// <list type="bullet">
    /// <item><description>Status Route: /status</description></item>
    /// </list>
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <returns>The route group builder.</returns>
    public static RouteGroupBuilder MapSolidusAuthenticationStatusAction(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(SessionController.ControllerRoute);

        group.MapAreaControllerRoute(
            name: "Solidus.Middleware.Authentication.SessionController.Status",
            areaName: ControllerHelpers.GetControllerArea<SessionController>(),
            pattern: "{action}",
            defaults: new
            {
                controller = ControllerHelpers.GetControllerRouteName<SessionController>()
            },
            constraints: new
            {
                action = ControllerHelpers.GetActionRouteConstraints<SessionController>(c => c.SessionStatus)
            }
        );

        return group;
    }

    /// <summary>
    /// Maps Solidus Authentication Sign Out action.
    /// <list type="bullet">
    /// <item><description>Sign Out Route: /sign-out</description></item>
    /// </list>
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <returns>The route group builder.</returns>
    public static RouteGroupBuilder MapSolidusAuthenticationSignOutAction(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(SessionController.ControllerRoute);

        group.MapAreaControllerRoute(
            name: "Solidus.Middleware.Authentication.SessionController.SignOut",
            areaName: ControllerHelpers.GetControllerArea<SessionController>(),
            pattern: "{action}",
            defaults: new
            {
                controller = ControllerHelpers.GetControllerRouteName<SessionController>()
            },
            constraints: new
            {
                action = ControllerHelpers.GetActionRouteConstraints<SessionController>(c => c.SessionSignOut)
            }
        );

        return group;
    }
}
