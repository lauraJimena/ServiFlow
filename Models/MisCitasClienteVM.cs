using ServiFlow.Models;

namespace ServiFlow.ViewModels
{
    public class MisCitasClienteVM
    {
        public int EmprendimientoId { get; set; }
        public string NombreEmprendimiento { get; set; } = string.Empty;

        public List<Cita> Citas { get; set; } = new();
        public string Orden { get; set; } = "cercana";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public int TotalItems { get; set; }
    }
}