using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace VideoGamesReboot24.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";
        private static readonly string[] roles = { "Admin", "User" };
        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            

            //Creating an Admin User
            AppIdentityDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<AppIdentityDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            EnsureRolesExist(app);
            UserManager<IdentityUser> userManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();
            IdentityUser user = await userManager.FindByNameAsync(adminUser);
            if (user == null)
            {
                user = new IdentityUser("Admin");
                user.Email = "admin@example.com";
                user.PhoneNumber = "555-1234";
                await userManager.CreateAsync(user, adminPassword);
            }
            IList<string> roles = await userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        //Creating the rolls in the database if they dont exist already
        private static async void EnsureRolesExist(IApplicationBuilder app)
        {
            RoleManager<IdentityRole> roleManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
