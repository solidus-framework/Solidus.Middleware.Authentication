using System.Net;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Solidus.Test;

// TODO: Move to common integration testing library.

public abstract class AspNetCoreIntegrationTestBase : TestBase
{
    private WebApplication? _app;
    private HttpClient? _client;

    protected HttpClient Client => _client ?? throw new InvalidOperationException("Web Application Client is not initialized.");

    [OneTimeSetUp]
    protected async Task OneTimeSetUp_StartWebApplication()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseKestrel(o => o.Listen(IPAddress.Loopback, 0));

        SetupServices(builder.Services);

        _app = builder.Build();

        SetupWebApplication(_app);

        await _app.StartAsync();
    }

    [OneTimeTearDown]
    protected async Task OneTimeTearDown_DisposeWebApplication()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
            _app = null;
        }
    }

    [SetUp]
    protected void SetUp_CreateHttpClient()
    {
        if (_app == null)
        {
            throw new InvalidOperationException("Web Application Server is not set up.");
        }

        var server = _app.Services.GetRequiredService<IServer>();
        var address = (server.Features.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault())
            ?? throw new InvalidOperationException("Unable to resolve Web Application Server address.");

        _client = new HttpClient
        {
            BaseAddress = new Uri(address),
        };
    }

    [TearDown]
    protected void TearDown_DisposeHttpClient()
    {
        if (_client != null)
        {
            _client.Dispose();
            _client = null;
        }
    }

    protected abstract void SetupServices(IServiceCollection services);

    protected abstract void SetupWebApplication(WebApplication app);
}
