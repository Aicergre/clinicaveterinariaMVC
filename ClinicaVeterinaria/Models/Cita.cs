using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Models
{
    public class Cita : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de la cita")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }

        public Mascota? Mascota { get; set; }

        [Required]
        [Display(Name = "Servicio Veterinario")]
        public int ServicioVeterinarioId { get; set; }

        public ServicioVeterinario? ServicioVeterinario { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Fecha.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha no puede ser anterior a la fecha actual.",
                    new[] { nameof(Fecha) });
            }
        }
    }
}