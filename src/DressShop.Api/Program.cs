using System.Security.Claims;
using DressShop.Api.Extensions;
using DressShop.Api.Middleware;
using DressShop.Application;
using DressShop.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = builder.Configuration["Supabase:Url"]?.TrimEnd('/');

if (string.IsNullOrWhiteSpace(supabaseUrl))
{
    throw new InvalidOperationException(
        "Supabase:Url is required.");
}

var supabaseIssuer = $"{supabaseUrl}/auth/v1";

// -----------------------------------------------------------------------------
// Services
// -----------------------------------------------------------------------------

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = supabaseIssuer;
        options.Audience = "authenticated";
        options.RequireHttpsMetadata = true;

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = supabaseIssuer,

            ValidateAudience = true,
            ValidAudience = "authenticated",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// AddBearerSecurityScheme() declares the Bearer scheme in the OpenAPI
// document itself - without it, Scalar has no "Authorize" button and
// manually-typed Authorization headers in its test client are unreliable.
builder.Services.AddOpenApi(options => options.AddBearerSecurityScheme());

// -----------------------------------------------------------------------------
// App
// -----------------------------------------------------------------------------



var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();
app.MapScalarApiReference();
//}

app.UseExceptionHandler();

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/user", (ClaimsPrincipal principal) =>
{
    var claims = principal.Claims.ToDictionary(
        c => c.Type,
        c => c.Value
    );

    return Results.Ok(claims);
})
.RequireAuthorization();

app.Run();

public partial class Program;
