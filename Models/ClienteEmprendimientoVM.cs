namespace ServiFlow.Models
{
    public class ClienteEmprendimientoVM
    {
        public int EmprendimientoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? TipoServicio { get; set; }

        public string? Descripcion { get; set; }

        public string? LogoUrl { get; set; }

        public string? BannerUrl { get; set; }

        public List<Servicio> Servicios { get; set; } = new();

        public double PromedioCalificacion { get; set; }

        public int TotalCalificaciones { get; set; }

        public int? MiCalificacion { get; set; }
    }
}