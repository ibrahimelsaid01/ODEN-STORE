using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreOde.Infrastructure.Email;
using StoreOde.Models;

namespace StoreOde.Extensions;

public static class ServiceCollectionExtensions
{
    public const string AdminRoleName = "Admin";
    public const string AdminPolicyName = "AdminOnly";

    public static IServiceCollection AddStoreOdeServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        AddDatabase(
            services,
            configuration,
            environment);

        AddIdentityServices(services);

        AddAuthorizationServices(services);

        AddEmailServices(
            services,
            configuration);

        AddForwardedHeaders(services);

        AddMvcServices(services);

        return services;
    }

    private static void AddDatabase(
        IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString =
            configuration.GetConnectionString(
                "SouqcomContext")
            ?? throw new InvalidOperationException(
                "Connection string 'SouqcomContext' was not found.");

        services.AddDbContext<SouqcomContext>(
            options =>
            {
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions
                            .EnableRetryOnFailure();
                    });
            });

        if (environment.IsDevelopment())
        {
            services
                .AddDatabaseDeveloperPageExceptionFilter();
        }
    }

    private static void AddIdentityServices(
        IServiceCollection services)
    {
        services
            .AddIdentity<IdentityUser, IdentityRole>(
                options =>
                {
                    options.SignIn
                        .RequireConfirmedAccount = true;

                    options.SignIn
                        .RequireConfirmedEmail = true;

                    options.User
                        .RequireUniqueEmail = true;

                    /*
                     * Preserve the Identity key length used
                     * by the existing database schema and
                     * previous migrations.
                     */
                    options.Stores
                        .MaxLengthForKeys = 128;

                    options.Password
                        .RequiredLength = 10;

                    options.Password
                        .RequiredUniqueChars = 1;

                    options.Password
                        .RequireDigit = true;

                    options.Password
                        .RequireLowercase = true;

                    options.Password
                        .RequireUppercase = true;

                    options.Password
                        .RequireNonAlphanumeric = false;

                    options.Lockout
                        .AllowedForNewUsers = true;

                    options.Lockout
                        .MaxFailedAccessAttempts = 5;

                    options.Lockout
                        .DefaultLockoutTimeSpan =
                        TimeSpan.FromMinutes(15);
                })
            .AddEntityFrameworkStores<SouqcomContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        services.ConfigureApplicationCookie(
            options =>
            {
                options.Cookie.HttpOnly = true;

                options.Cookie.SecurePolicy =
                    CookieSecurePolicy.Always;

                options.Cookie.SameSite =
                    SameSiteMode.Lax;

                options.ExpireTimeSpan =
                    TimeSpan.FromMinutes(60);

                options.SlidingExpiration = true;

                options.LoginPath =
                    "/Identity/Account/Login";

                options.AccessDeniedPath =
                    "/Identity/Account/AccessDenied";
            });
    }

    private static void AddAuthorizationServices(
        IServiceCollection services)
    {
        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    AdminPolicyName,
                    policy =>
                    {
                        policy.RequireRole(
                            AdminRoleName);
                    });
            });
    }

    private static void AddEmailServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<SmtpEmailOptions>()
            .Bind(
                configuration.GetSection(
                    SmtpEmailOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options =>
                    Enum.IsDefined(
                        options.SecurityMode),
                "The configured SMTP security mode is invalid.")
            .Validate(
                options =>
                    !options.RequireAuthentication ||
                    options.SecurityMode !=
                    SmtpSecurityMode.None,
                "SMTP authentication requires an encrypted connection.")
            .ValidateOnStart();

        services.AddTransient<
            IEmailSender,
            SmtpEmailSender>();
    }

    private static void AddForwardedHeaders(
        IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(
            options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;
            });
    }

    private static void AddMvcServices(
        IServiceCollection services)
    {
        services.AddControllersWithViews(
            options =>
            {
                options.Filters.Add(
                    new AutoValidateAntiforgeryTokenAttribute());
            });

        services.AddRazorPages();
    }
}