namespace ClinicaVeterinaria.Models
{

    public class DashboardViewModel
    {
        public int TotalServicios { get; set; }
        public int TotalMascotas { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalCitas { get; set; }

        public int CitasPendientes { get; set; }
        public int CitasAtendidas { get; set; }
        public int CitasCanceladas { get; set; }

        public List<string> Meses { get; set; } = new();
        public List<int> CantidadCitas { get; set; } = new();

        public List<Cita> UltimasCitas { get; set; } = new();
    }

}
