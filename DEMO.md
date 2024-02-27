# Solidus Authentication middleware usage example

## Initialize empty AspNetCore web project

```sh
dotnet new web
```

## Add Solidus Authentication middleware package

```sh
dotnet add package Solidus.Middleware.Authentication
```

## Add Entity Framework Core SQLite package

```sh
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

## Create account entity

`./Account.cs`

```csharp
using System.ComponentModel.DataAnnotations;

public class Account
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool IsAdmin { get; set; }
    public bool IsDeleted { get; set; }
}
```

## Create Entity Framework Core `DbContext` with an Account entity `DbSet`

`./ApplicationDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasIndex(e => e.Name)
            .IsUnique();
    }
}
```

## Create an Account entity adapter

`./AccountAdapter.cs`

```csharp
using Solidus.Middleware.Authentication.Account;

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
```

## Register services and endpoints

`./Program.cs`

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Solidus.Middleware.Authentication;
using Solidus.Middleware.Authentication.Account;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Listen(IPAddress.Loopback, 44019));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // In-memory SQLite database
    var sqliteConnection = new SqliteConnection("DataSource=file::memory:?cache=shared");
    options.UseSqlite(sqliteConnection);
});

builder.Services.AddAuthentication().AddSolidus(s =>
{
    s.Options.ChallengeUrl = "/login"; // Web application login page address

    // Add accounts support
    s.AddAccounts<Account>()
        // Add Entity Framework Core account storage
        .AddEntityFrameworkCoreStorage<ApplicationDbContext, AccountAdapter>(context => context.Accounts);
});

builder.Services.AddControllers()
    // Register authentication controllers
    .AddSolidusAuthenticationControllers();

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IAccountPasswordHasher>();
    dbContext.Database.EnsureCreated();

    // Add admin account if there are no admins in the database
    if (!dbContext.Accounts.Any(a => a.IsAdmin))
    {
        dbContext.Accounts.Add(new Account
        {
            Name = "admin",
            PasswordHash = passwordHasher.HashAccountPasswordAsync("admin", CancellationToken.None).Result,
            IsAdmin = true,
        });
        dbContext.SaveChanges();
    }
}

// Add required authentication and optional authorization middleware
app.UseAuthentication();
app.UseAuthorization();

var authGroup = app.MapGroup("/auth");
authGroup.MapSolidusAuthenticationStatusAction(); // path: /auth/status
authGroup.MapSolidusAuthenticationSignOutAction(); // path: /auth/sign-out
authGroup.MapSolidusAccountSignInAction(); // path: /auth/account/sign-in
authGroup.MapSolidusAccountSignUpAction(); // path: /auth/account/sign-up
// To restrict account registration configure policy authorization for Sign Up endpoint
// authGroup.MapSolidusAccountSignUpAction().RequireAuthorization();

app.Run();
```

## Run application

```sh
dotnet run
```

## Test application using postman

Collection to import:

```json
{
	"info": {
		"_postman_id": "9d489198-6bfd-4df0-bf4f-b764117a8b66",
		"name": "Solidus.Middleware.Authentication",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json",
		"_exporter_id": "18686893"
	},
	"item": [
		{
			"name": "sign-up",
			"request": {
				"method": "POST",
				"header": [],
				"body": {
					"mode": "raw",
					"raw": "{\r\n    \"name\": \"the-login\",\r\n    \"password\": \"the-password\",\r\n    \"rememberMe\": true,\r\n    \"metadata\": {\r\n        \"FirstName\": \"John\",\r\n        \"LastName\": \"Smith\"\r\n    }\r\n}",
					"options": {
						"raw": {
							"language": "json"
						}
					}
				},
				"url": {
					"raw": "{{url}}/auth/account/sign-up",
					"host": [
						"{{url}}"
					],
					"path": [
						"auth",
						"account",
						"sign-up"
					]
				}
			},
			"response": []
		},
		{
			"name": "sign-in",
			"request": {
				"method": "POST",
				"header": [],
				"body": {
					"mode": "raw",
					"raw": "{\r\n    \"name\": \"the-login\",\r\n    \"password\": \"the-password\",\r\n    \"rememberMe\": true\r\n}",
					"options": {
						"raw": {
							"language": "json"
						}
					}
				},
				"url": {
					"raw": "{{url}}/auth/account/sign-in",
					"host": [
						"{{url}}"
					],
					"path": [
						"auth",
						"account",
						"sign-in"
					]
				}
			},
			"response": []
		},
		{
			"name": "sign-out",
			"request": {
				"method": "POST",
				"header": [],
				"url": {
					"raw": "{{url}}/auth/sign-out",
					"host": [
						"{{url}}"
					],
					"path": [
						"auth",
						"sign-out"
					]
				}
			},
			"response": []
		},
		{
			"name": "status",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "{{url}}/auth/status",
					"host": [
						"{{url}}"
					],
					"path": [
						"auth",
						"status"
					]
				}
			},
			"response": []
		}
	],
	"event": [
		{
			"listen": "prerequest",
			"script": {
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		},
		{
			"listen": "test",
			"script": {
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		}
	],
	"variable": [
		{
			"key": "url",
			"value": "http://127.0.0.1:44019",
			"type": "string"
		}
	]
}
```
