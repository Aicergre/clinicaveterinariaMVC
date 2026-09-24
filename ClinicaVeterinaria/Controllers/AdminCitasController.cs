using ClinicaVeterinaria.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminCitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? estado,
            string? buscar)
        {
            var citas = _context.Citas
                .Include(c => c.Mascota)
                .ThenInclude(m => m.Usuario)
                .Include(c => c.ServicioVeterinario)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                citas = citas
                    .Where(c => c.Estado == estado);
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                citas = citas.Where(c =>
                    c.Mascota!.Nombre.Contains(buscar) ||
                    c.ServicioVeterinario!.Nombre.Contains(buscar));
            }

            return View(
                await citas
                    .OrderByDescending(c => c.Fecha)
                    .ToListAsync()
            );
        }


        public async Task<IActionResult> Details(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .ThenInclude(m => m.Usuario)
                .Include(c => c.ServicioVeterinario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
                return NotFound();

            return View(cita);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string estado)
        {
            var cita =
                await _context.Citas.FindAsync(id);

            if (cita == null)
                return NotFound();

            string[] estadosPermitidos =
            {
                "Pendiente",
                "Atendida",
                "Cancelada"
            };

            if (!estadosPermitidos.Contains(estado))
            {
                return BadRequest();
            }

            cita.Estado = estado;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Estado actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
