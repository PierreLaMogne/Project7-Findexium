using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Security.Claims;

namespace FindexiumAPI.logs
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ClaimType EXACT de votre JWT
            var nameIdClaimType = ClaimTypes.NameIdentifier; 
            // = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            
            var nameId = context.User.FindFirst(nameIdClaimType)?.Value;

            using (LogContext.PushProperty("UserNameId", nameId ?? "Anonymous"))
            {
                await _next(context);
            }
        }
    }
}