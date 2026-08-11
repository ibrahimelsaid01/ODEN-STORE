using Microsoft.AspNetCore.Identity;

namespace StoreOde.Infrastructure.Identity;

public static class AdminRoleInitializer
{
    private const string AdminRoleName = "Admin";
    private const string AdminEmailConfigurationKey = "Admin:Email";

    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var loggerFactory = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>();

        var logger = loggerFactory.CreateLogger(
            nameof(AdminRoleInitializer));

        await EnsureAdminRoleExistsAsync(
            roleManager,
            logger);

        var adminEmail = configuration[
            AdminEmailConfigurationKey];

        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            logger.LogInformation(
                "No administrator email is configured. " +
                "The Admin role was created, but no user was assigned to it.");

            return;
        }

        adminEmail = adminEmail.Trim();

        var adminUser = await userManager.FindByEmailAsync(
            adminEmail);

        if (adminUser is null)
        {
            logger.LogWarning(
                "The configured administrator account does not exist. " +
                "Register the configured account before assigning the Admin role.");

            return;
        }

        if (!adminUser.EmailConfirmed)
        {
            logger.LogWarning(
                "The configured administrator account has not confirmed its email address.");

            return;
        }

        if (await userManager.IsInRoleAsync(
                adminUser,
                AdminRoleName))
        {
            return;
        }

        var addToRoleResult =
            await userManager.AddToRoleAsync(
                adminUser,
                AdminRoleName);

        if (addToRoleResult.Succeeded)
        {
            logger.LogInformation(
                "The configured administrator account was assigned to the Admin role.");

            return;
        }

        var errors = string.Join(
            "; ",
            addToRoleResult.Errors.Select(
                error => error.Description));

        throw new InvalidOperationException(
            $"The configured administrator account could not be assigned " +
            $"to the '{AdminRoleName}' role. {errors}");
    }

    private static async Task EnsureAdminRoleExistsAsync(
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        if (await roleManager.RoleExistsAsync(
                AdminRoleName))
        {
            return;
        }

        var createRoleResult =
            await roleManager.CreateAsync(
                new IdentityRole(AdminRoleName));

        if (createRoleResult.Succeeded)
        {
            logger.LogInformation(
                "The Admin role was created successfully.");

            return;
        }

        var errors = string.Join(
            "; ",
            createRoleResult.Errors.Select(
                error => error.Description));

        throw new InvalidOperationException(
            $"The required role '{AdminRoleName}' " +
            $"could not be created. {errors}");
    }
}