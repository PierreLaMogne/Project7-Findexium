using FindexiumAPI.Common;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class AuthServiceTestHelper
{
    public static IServiceScope CreateCleanScope()
    {
        var services = new ServiceCollection();

        // Each test gets a unique in-memory database instance to ensure isolation and prevent data leakage between tests.
        var dbName = $"AuthTestDb_{Guid.NewGuid():N}[12]";
        services.AddDbContext<LocalDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        // Configure JWT settings with a long secret key for testing purposes.
        services.Configure<JwtSettings>(options =>
        {
            options.SecretKey = "ThisIsASuperSecretKeyForTestingThatIsLongerThan256Bits!!";
            options.Issuer = "TestIssuer";
            options.Audience = "TestAudience";
            options.ExpirationInMinutes = 60;
        });

        // Add Data Protection services required by Identity's token providers
        services.AddDataProtection();

        // Identity configuration with strong password requirements for testing.
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddEntityFrameworkStores<LocalDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IAuthService, AuthService>();

        // Build the service provider and create a scope for the test.
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();

        InitializeDatabaseAsync(scope).Wait();

        return scope;
    }

    private static async Task InitializeDatabaseAsync(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
