using ClinicaVeterinaria.Data;
using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =====================================================
        // MIS CITAS
        // =====================================================

        public async Task<IActionResult> Index()
        {
            string? usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
            {
                return Unauthorized();
            }

            var citas = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .Where(c =>
                    c.Mascota != null &&
                    c.Mascota.UsuarioId == usuarioId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return View(citas);
        }


        // =====================================================
        // SOLICITAR CITA - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string? usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
            {
                return Unauthorized();
            }

            bool tieneMascotas = await _context.Mascotas
                .AnyAsync(m => m.UsuarioId == usuarioId);

            if (!tieneMascotas)
            {
                TempData["Error"] =
                    "Primero debes registrar una mascota.";

                return RedirectToAction(
                    "Index",
                    "Mascotas"
                );
            }

            bool tieneServicios =
                await _context.ServiciosVeterinarios.AnyAsync();

            if (!tieneServicios)
            {
                TempData["Error"] =
                    "Actualmente no existen servicios veterinarios disponibles.";

                return RedirectToAction(nameof(Index));
            }

            await CargarCombos(usuarioId);

            var modelo = new CitaCreateViewModel
            {
                Fecha = DateTime.Today.AddDays(1),
                Hora = new TimeSpan(9, 0, 0)
            };

            return View(modelo);
        }


        // =====================================================
        // SOLICITAR CITA - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CitaCreateViewModel modelo)
        {
            string? usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
            {
                return Unauthorized();
            }

            DateTime fechaHora =
                modelo.Fecha.Date.Add(modelo.Hora);


            // Validar fecha futura
            if (fechaHora <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(modelo.Fecha),
                    "Debes seleccionar una fecha y hora futura."
                );
            }


            // Horario de atención
            TimeSpan horaInicio = new TimeSpan(8, 0, 0);
            TimeSpan horaFin = new TimeSpan(18, 0, 0);

            if (modelo.Hora < horaInicio ||
                modelo.Hora > horaFin)
            {
                ModelState.AddModelError(
                    nameof(modelo.Hora),
                    "El horario de atención es de 08:00 a 18:00."
                );
            }


            // Verificar que la mascota sea del cliente
            bool mascotaValida = await _context.Mascotas
                .AnyAsync(m =>
                    m.Id == modelo.MascotaId &&
                    m.UsuarioId == usuarioId);

            if (!mascotaValida)
            {
                ModelState.AddModelError(
                    nameof(modelo.MascotaId),
                    "La mascota seleccionada no es válida."
                );
            }


            // Verificar servicio
            bool servicioValido =
                await _context.ServiciosVeterinarios
                    .AnyAsync(s =>
                        s.Id == modelo.ServicioVeterinarioId);

            if (!servicioValido)
            {
                ModelState.AddModelError(
                    nameof(modelo.ServicioVeterinarioId),
                    "El servicio seleccionado no es válido."
                );
            }


            if (!ModelState.IsValid)
            {
                await CargarCombos(usuarioId);

                return View(modelo);
            }


            var cita = new Cita
            {
                MascotaId = modelo.MascotaId,

                ServicioVeterinarioId =
                    modelo.ServicioVeterinarioId,

                Fecha = fechaHora,

                Estado = "Pendiente"
            };

            _context.Citas.Add(cita);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Tu cita fue solicitada correctamente.";

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // DETALLE DE CITA
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            string? usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
            {
                return Unauthorized();
            }

            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Mascota != null &&
                    c.Mascota.UsuarioId == usuarioId);

            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }


        // =====================================================
        // CANCELAR CITA
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            string? usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
            {
                return Unauthorized();
            }

            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Mascota != null &&
                    c.Mascota.UsuarioId == usuarioId);

            if (cita == null)
            {
                return NotFound();
            }


            if (cita.Estado == "Cancelada")
            {
                TempData["Error"] =
                    "Esta cita ya se encuentra cancelada.";

                return RedirectToAction(nameof(Index));
            }


            if (cita.Estado == "Atendida")
            {
                TempData["Error"] =
                    "Una cita atendida ya no puede cancelarse.";

                return RedirectToAction(nameof(Index));
            }


            cita.Estado = "Cancelada";

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "La cita fue cancelada correctamente.";

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // CARGAR MASCOTAS Y SERVICIOS
        // =====================================================

        private async Task CargarCombos(string usuarioId)
        {
            var mascotas = await _context.Mascotas
                .Where(m => m.UsuarioId == usuarioId)
                .OrderBy(m => m.Nombre)
                .ToListAsync();


            var servicios =
                await _context.ServiciosVeterinarios
                    .OrderBy(s => s.Nombre)
                    .ToListAsync();


            ViewBag.Mascotas =
                new SelectList(
                    mascotas,
                    "Id",
                    "Nombre"
                );


            ViewBag.Servicios =
                new SelectList(
                    servicios,
                    "Id",
                    "Nombre"
                );
        }
    }
}