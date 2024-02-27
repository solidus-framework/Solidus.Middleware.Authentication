namespace Solidus.Middleware.Authentication.Account;

public static class SolidusAccountAuthenticationMvcBuilderExtensions
{
    /// <summary>
    /// Registers Solidus Authentication account-related assembly controllers.
    /// </summary>
    /// <param name="builder">An builder instance.</param>
    /// <returns>Same builder instance.</returns>
    public static IMvcBuilder AddSolidusAccountAuthenticationControllers(this IMvcBuilder builder)
    {
        builder.AddApplicationPart(typeof(AccountController).Assembly);

        return builder;
    }
}
