using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public static class ControllerTestExtensions
{
    public static void InitializeClaims(this ControllerBase controller, params Claim[] claims)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    public static void InitializeAdminUser(this ControllerBase controller)
    {
        controller.InitializeClaims(
            new Claim(ClaimTypes.NameIdentifier, "test-admin-id"),
            new Claim(ClaimTypes.Name, "testAdmin"),
            new Claim(ClaimTypes.Role, "Admin")
        );
    }

    public static void InitializeRegularUser(this ControllerBase controller)
    {
        controller.InitializeClaims(
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Name, "testUser"),
            new Claim(ClaimTypes.Role, "User")
        );
    }

    public static void InitializeUnauthenticatedUser(this ControllerBase controller)
    {
        controller.InitializeClaims(
            new Claim(ClaimTypes.NameIdentifier, "test-norole-id"),
            new Claim(ClaimTypes.Name, "testNoRole")
        );
    }

    /// <summary>
    /// Checks if the user satisfies the "Users" policy (requires User or Admin role).
    /// Returns true if authorized, false otherwise.
    /// Use this to simulate [Authorize(Policy = "Users")] in unit tests.
    /// </summary>
    public static bool IsAuthorizedForUsersPolicy(this ControllerBase controller)
    {
        if (!controller.User.Identity?.IsAuthenticated ?? true)
            return false;

        return controller.User.IsInRole("User") || controller.User.IsInRole("Admin");
    }

    /// <summary>
    /// Checks if the user has the Admin role.
    /// Returns true if authorized, false otherwise.
    /// Use this to simulate [Authorize(Roles = "Admin")] in unit tests.
    /// </summary>
    public static bool IsAuthorizedAsAdmin(this ControllerBase controller)
    {
        if (!controller.User.Identity?.IsAuthenticated ?? true)
            return false;

        return controller.User.IsInRole("Admin");
    }
}