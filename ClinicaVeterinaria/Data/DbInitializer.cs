using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Identity;

namespace ClinicaVeterinaria.Data
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(
            IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "Administrador",
                "Cliente"
            };

            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(rol));
                }
            }

            string correoAdmin = "admin@veterinaria.com";

            var admin =
                await userManager.FindByEmailAsync(correoAdmin);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = correoAdmin,
                    Email = correoAdmin,
                    NombreCompleto =
                        "Administrador Veterinaria",
                    EmailConfirmed = true
                };

                var resultado =
                    await userManager.CreateAsync(
                        admin,
                        "Admin123!");

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Administrador");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(
                    admin,
                    "Administrador"))
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Administrador");
                }
            }
        }
    }
}
