using DressShop.Api.Authorization;
using DressShop.Api.Extensions;
using DressShop.Api.Middleware;
using DressShop.Api.Services;
using DressShop.Application;
using DressShop.Application.Abstractions;
using DressShop.Infrastructure;
using DressShop.Infrastructure.Persistence;
using DressShop.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new AdminRequirement());
    });
});

builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// DI registration
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// AddBearerSecurityScheme() declares the Bearer scheme in the OpenAPI
// document itself - without it, Scalar has no "Authorize" button and
// manually-typed Authorization headers in its test client are unreliable.
builder.Services.AddOpenApi(options => options.AddBearerSecurityScheme());

builder.Services.AddHttpContextAccessor();


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
    _ = app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
