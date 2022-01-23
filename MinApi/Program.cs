using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinApi;
using MinApi.Models;
using MinApi.Repositories;
using MinApi.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("Admin", policiy => policiy.RequireRole("manager"));
    x.AddPolicy("Employee", policiy => policiy.RequireRole("employee"));
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (User model) =>
{
    var user = UserRepositorie.Get(model.UserName, model.Password);

    if (user == null)
        return Results.NotFound(new { message = "Invalid User name or Password" });

    var token = TokenServices.GenerateToken(user);

    user.Password = String.Empty;

    return Results.Ok(new
    {
        user = user,
        token = token
    });
});

app.MapGet("authenticated", (ClaimsPrincipal user) =>
 {
     Results.Ok(new { message = $"Autenticated as {user.Identity.Name}" });
 }).RequireAuthorization();


app.MapGet("/manager", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Autenticated as {user.Identity.Name}" });
}).RequireAuthorization("Admin");

app.MapGet("/Employee", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Autenticated as {user.Identity.Name}" });
}).RequireAuthorization("Employee");


app.Run();

