using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliUteco.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Crear roles si no existen
            string[] roleNames = { "Administrador", "Bibliotecario" };
            // string[] roleNames = { "Administrador", "Bibliotecario", "Lector" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Crear un usuario administrador por defecto si no existe
            var adminEmail = "admin@bibliuteco.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123!");

                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Administrador");
                }
            }
        }
    }
}