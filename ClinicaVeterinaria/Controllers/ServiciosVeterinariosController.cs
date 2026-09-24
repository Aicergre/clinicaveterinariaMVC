using ClinicaVeterinaria.Data;
using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize]
    public class ServiciosVeterinariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiciosVeterinariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ADMIN Y CLIENTE
        public async Task<IActionResult> Index()
        {
            var servicios = await _context.ServiciosVeterinarios
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return View(servicios);
        }

        // ADMIN Y CLIENTE
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var servicio = await _context.ServiciosVeterinarios
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        // SOLO ADMIN
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(ServicioVeterinario servicio)
        {
            if (ModelState.IsValid)
            {
                _context.ServiciosVeterinarios.Add(servicio);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Servicio registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(servicio);
        }

        // SOLO ADMIN
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var servicio = await _context.ServiciosVeterinarios.FindAsync(id);

            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, ServicioVeterinario servicio)
        {
            if (id != servicio.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "Servicio actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioExiste(servicio.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(servicio);
        }

        // SOLO ADMIN
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var servicio = await _context.ServiciosVeterinarios
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicio = await _context.ServiciosVeterinarios.FindAsync(id);

            if (servicio == null)
                return NotFound();

            bool tieneCitas = await _context.Citas
                .AnyAsync(c => c.ServicioVeterinarioId == id);

            if (tieneCitas)
            {
                TempData["Error"] =
                    "No se puede eliminar porque el servicio tiene citas registradas.";

                return RedirectToAction(nameof(Index));
            }

            _context.ServiciosVeterinarios.Remove(servicio);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Servicio eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private bool ServicioExiste(int id)
        {
            return _context.ServiciosVeterinarios.Any(s => s.Id == id);
        }
    }
}
