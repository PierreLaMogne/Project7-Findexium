using FindexiumAPI.Common;
using FindexiumAPI.Controllers;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.Extensions;
using System.Security.Claims;

namespace FindexiumAPI.Tests.ControllersTests
{
    public class AuthControllerTestHelper
    {
        public static (AuthController controller, IServiceScope scope) CreateControllerScope()
        {
            var services = new ServiceCollection();

            // Configurating in-memory database for testing
            services.AddDbContext<LocalDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            // Data Protection for token generation
            services.AddDataProtection();

            // Configuring Identity services
            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<LocalDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(options =>
            {
                options.SecretKey = "ThisIsASecretKeyForTestingPurposesOnly!";
                options.Issuer = "TestIssuer";
                options.Audience = "TestAudience";
                options.ExpirationInMinutes = 60;
            });

            services.AddScoped<IAuthService, AuthService>();
            services.AddControllers();

            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();

            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            var controller = new AuthController(authService);

            return (controller, scope);
        }

        // Helper method to create an authenticated ControllerContext with specified user claims
        public static ControllerContext CreateAuthenticatedContext(string userId, string role = "User")
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, $"user{userId}"),
            new Claim(ClaimTypes.Role, role)
        };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };
            return new ControllerContext { HttpContext = httpContext };
        }
    }
}