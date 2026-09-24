using ClinicaVeterinaria.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // REPORTE 1: LISTADO GENERAL DE CITAS
        public async Task<IActionResult> CitasGenerales()
        {
            var citas = await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.Usuario)
                .Include(c => c.ServicioVeterinario)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("VetCare - Listado General de Citas")
                        .FontSize(20)
                        .Bold();

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm}");

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Mascota").Bold();
                                header.Cell().Text("Cliente").Bold();
                                header.Cell().Text("Servicio").Bold();
                                header.Cell().Text("Fecha").Bold();
                                header.Cell().Text("Estado").Bold();
                            });

                            foreach (var cita in citas)
                            {
                                table.Cell()
                                    .Text(cita.Mascota?.Nombre ?? "-");

                                table.Cell()
                                    .Text(cita.Mascota?.Usuario?.NombreCompleto ?? "-");

                                table.Cell()
                                    .Text(cita.ServicioVeterinario?.Nombre ?? "-");

                                table.Cell()
                                    .Text(cita.Fecha.ToString("dd/MM/yyyy"));

                                table.Cell()
                                    .Text(cita.Estado);
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                        });
                });
            });

            byte[] pdf = documento.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "Listado_General_Citas.pdf"
            );
        }


        // REPORTE 2: SERVICIOS MÁS SOLICITADOS
        public async Task<IActionResult> ServiciosSolicitados()
        {
            var servicios = await _context.ServiciosVeterinarios
                .Select(s => new
                {
                    s.Nombre,
                    s.Precio,
                    CantidadCitas = s.Citas.Count()
                })
                .OrderByDescending(s => s.CantidadCitas)
                .ToListAsync();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("VetCare - Servicios Más Solicitados")
                        .FontSize(20)
                        .Bold();

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm}");

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Text("Servicio")
                                    .Bold();

                                header.Cell()
                                    .Text("Precio")
                                    .Bold();

                                header.Cell()
                                    .Text("Cantidad de citas")
                                    .Bold();
                            });

                            foreach (var servicio in servicios)
                            {
                                table.Cell()
                                    .Text(servicio.Nombre);

                                table.Cell()
                                    .Text($"Bs. {servicio.Precio:0.00}");

                                table.Cell()
                                    .Text(servicio.CantidadCitas.ToString());
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                        });
                });
            });

            byte[] pdf = documento.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "Servicios_Mas_Solicitados.pdf"
            );
        }
    }
}