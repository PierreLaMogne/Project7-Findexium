using FindexiumAPI.Common;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FindexiumAPI.Tests.ServicesTests
{
    public static class AuthServiceTestHelper
    {
        public static IServiceScope CreateCleanScope()
        {
            var services = new ServiceCollection();
            // Configure in-memory database
            services.AddDbContext<LocalDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            // Configure Identity
            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<LocalDbContext>();

            // Configure JwtSettings
            services.Configure<JwtSettings>(options =>
            {
                options.SecretKey = "ThisIsASecretKeyForTestingPurposesOnly!";
                options.Issuer = "TestIssuer";
                options.Audience = "TestAudience";
                options.ExpirationInMinutes = 60;
            });

            // Register AuthService
            services.AddScoped<IAuthService, AuthService>();
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.CreateScope();
        }
    }
}
