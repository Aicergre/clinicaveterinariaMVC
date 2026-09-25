using ClinicaVeterinaria.Data;
using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class MascotasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MascotasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===============================
        // MIS MASCOTAS
        // ===============================

        public async Task<IActionResult> Index()
        {
            var usuarioId = _userManager.GetUserId(User);

            var mascotas = await _context.Mascotas
                .Where(m => m.UsuarioId == usuarioId)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return View(mascotas);
        }

        // ===============================
        // REGISTRAR MASCOTA
        // ===============================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mascota mascota)
        {
            var usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
                return Unauthorized();

            mascota.UsuarioId = usuarioId;

            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");

            if (!ModelState.IsValid)
            {
                return View(mascota);
            }

            _context.Mascotas.Add(mascota);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Mascota registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // DETALLE
        // ===============================

        public async Task<IActionResult> Details(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.UsuarioId == usuarioId);

            if (mascota == null)
                return NotFound();

            return View(mascota);
        }

        // ===============================
        // EDITAR
        // ===============================

        public async Task<IActionResult> Edit(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.UsuarioId == usuarioId);

            if (mascota == null)
                return NotFound();

            return View(mascota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mascota modelo)
        {
            var usuarioId = _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.UsuarioId == usuarioId);

            if (mascota == null)
                return NotFound();

            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");

            if (!ModelState.IsValid)
                return View(modelo);

            mascota.Nombre = modelo.Nombre;
            mascota.Especie = modelo.Especie;
            mascota.Raza = modelo.Raza;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Mascota actualizada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // ELIMINAR
        // ===============================

        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.UsuarioId == usuarioId);

            if (mascota == null)
                return NotFound();

            return View(mascota);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.UsuarioId == usuarioId);

            if (mascota == null)
                return NotFound();

            _context.Mascotas.Remove(mascota);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Mascota eliminada correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}