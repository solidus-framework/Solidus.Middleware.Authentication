using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Solidus.Middleware.Authentication.IntegrationTests;

public class AuthenticationIntegrationTests : AspNetCoreIntegrationTestBase
{
    private const string AuthBaseRoute = "/auth";

    private static readonly string SignUpUrl = Path.Combine(
        AuthBaseRoute,
        AccountController.ControllerRoute,
        AccountController.AccountSignUpActionRoute
    );

    private static readonly string SignInUrl = Path.Combine(
        AuthBaseRoute,
        AccountController.ControllerRoute,
        AccountController.AccountSignInActionRoute
    );

    private static readonly string SignOutUrl = Path.Combine(
        AuthBaseRoute,
        SessionController.ControllerRoute,
        SessionController.SessionSignOutActionRoute
    );

    private static readonly string StatusUrl = Path.Combine(
        AuthBaseRoute,
        SessionController.ControllerRoute,
        SessionController.SessionStatusActionRoute
    );

    [Test]
    public async Task FullFlow()
    {
        // Arrange.
        const string name = "the-name";
        const string password = "the-password";

        AccountController.AccountSignUpRequestModel signUpModel = new()
        {
            Name = name,
            Password = password,
            RememberMe = true,
        };

        AccountController.AccountSignInRequestModel signInModel = new()
        {
            Name = name,
            Password = password,
            RememberMe = true,
        };

        // Act.
        var initialStatusResponse = await Client.GetAsync(StatusUrl);
        var initialSignOutResponse = await Client.PostAsync(SignOutUrl, null);
        var initialSignInResponse = await Client.PostAsJsonAsync(SignInUrl, signInModel);
        var initialSignUpResponse = await Client.PostAsJsonAsync(SignUpUrl, signUpModel);

        var signUpStatusResponse = await Client.GetAsync(StatusUrl);
        var signUpSignInResponse = await Client.PostAsJsonAsync(SignInUrl, signInModel);
        var signUpSignUpResponse = await Client.PostAsJsonAsync(SignUpUrl, signUpModel);
        var signUpSignOutResponse = await Client.PostAsync(SignOutUrl, null);

        var signOutStatusResponse = await Client.GetAsync(StatusUrl);
        var signOutSignOutResponse = await Client.PostAsync(SignOutUrl, null);
        var signOutSignInResponse = await Client.PostAsJsonAsync(SignInUrl, signInModel);
        var signOutSignInStatusResponse = await Client.GetAsync(StatusUrl);

        var initialStatus = initialStatusResponse.IsSuccessStatusCode
            ? await initialStatusResponse.Content.ReadFromJsonAsync<SessionController.SessionStatusResponseModel>()
            : null;


        var signUpStatus = signUpStatusResponse.IsSuccessStatusCode
            ? await signUpStatusResponse.Content.ReadFromJsonAsync<SessionController.SessionStatusResponseModel>()
            : null;

        var signOutStatus = signOutStatusResponse.IsSuccessStatusCode
            ? await signOutStatusResponse.Content.ReadFromJsonAsync<SessionController.SessionStatusResponseModel>()
            : null;

        var signOutSignInStatus = signOutSignInStatusResponse.IsSuccessStatusCode
            ? await signOutSignInStatusResponse.Content.ReadFromJsonAsync<SessionController.SessionStatusResponseModel>()
            : null;

        // Assert.
        Assert.That(initialStatusResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(initialSignOutResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(initialSignInResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(initialSignUpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        Assert.That(signUpStatusResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(signUpSignInResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(signUpSignUpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        Assert.That(signUpSignOutResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        Assert.That(signOutStatusResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(signOutSignOutResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(signOutSignInResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(signOutSignInStatusResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        Assert.That(initialStatus, Is.Null);

        Assert.That(signUpStatus, Is.Not.Null);
        Assert.That(signUpStatus.Claims.Keys.Contains("sub"));
        Assert.That(signUpStatus.Claims.Keys.Contains("role"));

        Assert.That(signOutStatus, Is.Null);

        Assert.That(signOutSignInStatus, Is.Not.Null);
        Assert.That(signOutSignInStatus.Claims.Keys.Contains("sub"));
        Assert.That(signOutSignInStatus.Claims.Keys.Contains("role"));
    }

    protected override void SetupServices(IServiceCollection services)
    {
        services.AddDbContext<AccountDbContext>(options =>
        {
            var sqliteConnection = new SqliteConnection("DataSource=file::memory:?cache=shared");
            options.UseSqlite(sqliteConnection);
        });

        services.AddAuthentication().AddSolidus(s =>
        {
            s.Options.ChallengeUrl = "/login";
            s.AddAccounts<Account>().AddEntityFrameworkCoreStorage<AccountDbContext, AccountAdapter>(context => context.Accounts);
        });

        services.AddControllers().AddSolidusAuthenticationControllers();
    }

    protected override void SetupWebApplication(WebApplication app)
    {
        app.Services.GetRequiredService<AccountDbContext>().Database.EnsureCreated();

        app.UseAuthentication();
        app.UseAuthorization();

        var authGroup = app.MapGroup(AuthBaseRoute);
        authGroup.MapSolidusAuthenticationStatusAction();
        authGroup.MapSolidusAuthenticationSignOutAction();
        authGroup.MapSolidusAccountSignUpAction();
        authGroup.MapSolidusAccountSignInAction();
    }

    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool IsDeleted { get; set; }
    }

    public class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(e => e.Name)
                .IsUnique();
        }
    }

    public class AccountAdapter : DbContextAccountAdapter<Account>
    {
        public override Account CreateEntity() => new();
        public override string GetId(Account account) => account.Id.ToString();
        public override string GetName(Account account) => account.Name;
        public override void SetName(Account account, string name) => account.Name = name;
        public override string GetPasswordHash(Account account) => account.PasswordHash;
        public override void SetPasswordHash(Account account, string passwordHash) => account.PasswordHash = passwordHash;
        public override IDictionary<string, string>? GetMetadata(Account account) => null;
        public override void SetMetadata(Account account, IDictionary<string, string>? metadata) {}
        public override bool GetIsDeleted(Account account) => account.IsDeleted;
        public override void SetIsDeleted(Account account, bool isDeleted) => account.IsDeleted = true;
        public override IQueryable<Account> FilterByNotDeleted(IQueryable<Account> query) => query;
        public override IQueryable<Account> FilterById(IQueryable<Account> query, string id) => query.Where(q => q.Id.ToString() == id);
        public override IQueryable<Account> FilterByName(IQueryable<Account> query, string name) => query.Where(q => q.Name == name);
    }
}
