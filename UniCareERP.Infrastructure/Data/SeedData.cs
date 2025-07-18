using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UniCareERP.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UniCareERP.Infrastructure.Data
{
    public class SeedDataRunner
    {
    }

    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, ILogger<SeedDataRunner> logger)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // Define roles to create
            string[] roleNames = {
                "Admin", "Doctor", "Nurse", "Pharmacist",
                "LabTechnician", "FinanceHead", "HRManager", "Receptionist"
            };

            string[] roleDescriptions = {
                "System Administrator with full access",
                "Medical Doctor",
                "Registered Nurse",
                "Pharmacy Staff",
                "Laboratory Technician",
                "Head of Finance Department",
                "Human Resources Manager",
                "Reception and Front Desk Staff"
            };

            for(int i=0; i < roleNames.Length; i++)
            {
                var roleName = roleNames[i];
                var roleDescription = roleDescriptions[i];
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName, roleDescription));
                    logger.LogInformation($"Role '{roleName}' created.");
                }
            }

            // Create Admin User
            string adminUserName = "admin@unicare.com";
            string adminPassword = "AdminPassword123!"; // Store this securely (e.g., config) for real apps

            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminUserName,
                    FirstName = "UniCare",
                    LastName = "Admin",
                    EmailConfirmed = true, // Auto-confirm admin email
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation($"User '{adminUserName}' created and assigned to 'Admin' role.");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        logger.LogError($"Error creating admin user: {error.Description}");
                    }
                }
            }
        }
    }
}
