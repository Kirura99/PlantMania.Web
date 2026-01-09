using Microsoft.AspNetCore.Identity;

namespace PlantMania.Web.Data;

public static class IdentitySeeder
{
    // Change these later or move to appsettings.json
    private const string AdminEmail = "admin@plantmania.local";
    private const string AdminPassword = "Admin123!";

    private static readonly string[] Roles =
    [
        "Admin",
        "Moderator",
        "User"
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // 1) Roles
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2) Admin user
        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, AdminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new InvalidOperationException($"Admin user creation failed: {errors}");
            }
        }

        // 3) Ensure Admin role
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}
