using DressShop.Application.Abstractions;
using DressShop.Infrastructure.Persistence;
using DressShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DressShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found in configuration.");



        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<AppDbContext>());

        var supabaseUrl = configuration["Supabase:Url"]?.TrimEnd('/')
            ?? throw new InvalidOperationException("Supabase:Url is required.");

        var supabaseAnonKey = configuration["Supabase:AnonKey"]
     ?? throw new InvalidOperationException("Supabase:AnonKey is required.");

        services.AddHttpClient<IProfilesClient, SupabaseProfilesClient>(client =>
        {
            client.BaseAddress = new Uri($"{supabaseUrl}/");
        });

        // Repositories, IEmailService, IFileStorageService, etc. get
        // registered here as they're introduced - see ARCHITECTURE.md
        // section 7 for why their interfaces live in Application, not here. 

        services.AddHttpClient<IAuthClient, Authentication.SupabaseAuthClient>(client =>
        {
            client.BaseAddress = new Uri($"{supabaseUrl}/auth/v1/");
            client.DefaultRequestHeaders.Add("apikey", supabaseAnonKey);
        });

        return services;
    }
}
