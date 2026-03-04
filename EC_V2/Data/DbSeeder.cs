using EC_V2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EC_V2.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Seed roles
            foreach (var role in new[] { "Admin", "Vendor", "Customer" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin
            var adminPhone = "+201000000000";
            var existingAdmin = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == adminPhone);

            if (existingAdmin == null)
            {
                var admin = new AppUser
                {
                    UserName = adminPhone,
                    PhoneNumber = adminPhone,
                    FirstName = "Admin",
                    LastName = "EC_V2",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
