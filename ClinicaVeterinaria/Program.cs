using ClinicaVeterinaria.Data;
using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// QUEST PDF
// ==========================================

QuestPDF.Settings.License = LicenseType.Community;


// ==========================================
// BASE DE DATOS
// ==========================================

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection"
    )
    ?? throw new InvalidOperationException(
        "No se encontró DefaultConnection."
    );

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(connectionString)
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// ==========================================
// IDENTITY
// ==========================================

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        // Para el proyecto no exigimos confirmación por correo
        options.SignIn.RequireConfirmedAccount = false;

        // Contraseña
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// ==========================================
// MVC
// ==========================================

builder.Services.AddControllersWithViews();

var app = builder.Build();


// ==========================================
// CONFIGURACIÓN HTTP
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();


// ==========================================
// CREAR ROLES Y USUARIOS INICIALES
// ==========================================

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InicializarAsync(
        scope.ServiceProvider
    );
}


// ==========================================
// EJECUTAR
// ==========================================

app.Run();