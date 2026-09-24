using ClinicaVeterinaria.Data;
using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var modelo = new DashboardViewModel
            {
                TotalServicios = await _context.ServiciosVeterinarios.CountAsync(),
                TotalMascotas = await _context.Mascotas.CountAsync(),
                TotalUsuarios = await _context.Users.CountAsync(),
                TotalCitas = await _context.Citas.CountAsync(),

                CitasPendientes = await _context.Citas.CountAsync(c => c.Estado == "Pendiente"),
                CitasAtendidas = await _context.Citas.CountAsync(c => c.Estado == "Atendida"),
                CitasCanceladas = await _context.Citas.CountAsync(c => c.Estado == "Cancelada"),

                UltimasCitas = await _context.Citas
                    .Include(c => c.Mascota)
                    .Include(c => c.ServicioVeterinario)
                    .OrderByDescending(c => c.Fecha)
                    .Take(5)
                    .ToListAsync()
            };

            var citasPorMes = await _context.Citas
                .GroupBy(c => new
                {
                    c.Fecha.Year,
                    c.Fecha.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Cantidad = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            foreach (var item in citasPorMes)
            {
                var fecha = new DateTime(item.Year, item.Month, 1);

                modelo.Meses.Add(
                    fecha.ToString("MMM yyyy"));

                modelo.CantidadCitas.Add(
                    item.Cantidad);
            }

            return View(modelo);
        }
    }
}
