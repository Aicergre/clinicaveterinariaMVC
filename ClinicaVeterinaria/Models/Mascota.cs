using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria")]
        [StringLength(50)]
        public string Especie { get; set; } = string.Empty;

        [Required(ErrorMessage = "La raza es obligatoria")]
        [StringLength(50)]
        public string Raza { get; set; } = string.Empty;

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        public ApplicationUser? Usuario { get; set; }

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}