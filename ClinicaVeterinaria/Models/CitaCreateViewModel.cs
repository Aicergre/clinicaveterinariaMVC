using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Models
{
    public class CitaCreateViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una mascota.")]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un servicio.")]
        [Display(Name = "Servicio veterinario")]
        public int ServicioVeterinarioId { get; set; }

        [Required(ErrorMessage = "Seleccione una fecha.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Seleccione una hora.")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora")]
        public TimeSpan Hora { get; set; }
    }
}