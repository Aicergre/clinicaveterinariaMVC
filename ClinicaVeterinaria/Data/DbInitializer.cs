using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Data
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            // ==========================================
            // CREAR ROLES
            // ==========================================

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
                        new IdentityRole(rol)
                    );
                }
            }


            // ==========================================
            // CREAR ADMINISTRADOR
            // ==========================================

            string correoAdmin = "admin@veterinaria.com";
            string passwordAdmin = "Admin123!";

            var admin =
                await userManager.FindByEmailAsync(correoAdmin);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = correoAdmin,
                    Email = correoAdmin,
                    NombreCompleto = "Administrador Veterinaria",
                    EmailConfirmed = true
                };

                var resultadoAdmin =
                    await userManager.CreateAsync(
                        admin,
                        passwordAdmin
                    );

                if (resultadoAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Administrador"
                    );
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
                        "Administrador"
                    );
                }
            }


            // ==========================================
            // CREAR CLIENTE DE PRUEBA
            // ==========================================

            string correoCliente =
                "cliente@veterinaria.com";

            string passwordCliente =
                "Cliente123!";

            var cliente =
                await userManager.FindByEmailAsync(
                    correoCliente
                );

            if (cliente == null)
            {
                cliente = new ApplicationUser
                {
                    UserName = correoCliente,
                    Email = correoCliente,
                    NombreCompleto = "Cliente Prueba",
                    EmailConfirmed = true
                };

                var resultadoCliente =
                    await userManager.CreateAsync(
                        cliente,
                        passwordCliente
                    );

                if (resultadoCliente.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        cliente,
                        "Cliente"
                    );
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(
                    cliente,
                    "Cliente"))
                {
                    await userManager.AddToRoleAsync(
                        cliente,
                        "Cliente"
                    );
                }
            }


            // ==========================================
            // ASIGNAR CLIENTE A USUARIOS SIN ROL
            // ==========================================
            // Esto arreglará automáticamente prueba1@gmail.com

            var usuarios =
                await userManager.Users.ToListAsync();

            foreach (var usuario in usuarios)
            {
                var rolesUsuario =
                    await userManager.GetRolesAsync(usuario);

                if (rolesUsuario.Count == 0)
                {
                    await userManager.AddToRoleAsync(
                        usuario,
                        "Cliente"
                    );
                }
            }
        }
    }
}