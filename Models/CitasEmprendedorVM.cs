using ServiFlow.Models;

namespace ServiFlow.ViewModels
{
    public class CitasEmprendedorVM
    {
        public int EmprendimientoId { get; set; }
        public string NombreEmprendimiento { get; set; } = string.Empty;

        public int? ServicioIdFiltro { get; set; }
        public DateTime? FechaFiltro { get; set; }

        public List<Servicio> Servicios { get; set; } = new();
        public List<Cita> Citas { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 3;
        public int TotalItems { get; set; }
    }
}