namespace Solidus.Middleware.Authentication.Account;

public static class SolidusAuthenticationMvcBuilderExtensions
{
    /// <summary>
    /// Registers Solidus Authentication assembly controllers.
    /// </summary>
    /// <param name="builder">An builder instance.</param>
    /// <returns>Same builder instance.</returns>
    public static IMvcBuilder AddSolidusAuthenticationControllers(this IMvcBuilder builder)
    {
        builder.AddApplicationPart(typeof(SessionController).Assembly);

        builder.AddSolidusAccountAuthenticationControllers();

        return builder;
    }
}
