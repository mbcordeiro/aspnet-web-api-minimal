using CatalogApi.Model;
using CatalogApi.Service;
using Microsoft.AspNetCore.Authorization;

namespace CatalogApi.ApiEndpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this WebApplication app)
        {
            app.MapPost("/login", [AllowAnonymous] (User user, ITokenService tokenService) =>
            {
                if (user == null) return Results.BadRequest("Invalid Login");
                if (user.Username == "mbcordeiro" && user.Password == "pass#123")
                {
                    var tokenString = tokenService.GenerateToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        user);
                    return Results.Ok(new { token = tokenString });
                }
                else return Results.BadRequest("Invalid Login");
            }).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .WithName("Login")
              .WithTags("Authentication");
        }
    }
}
