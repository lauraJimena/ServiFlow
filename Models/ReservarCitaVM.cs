using ServiFlow.Models;

namespace ServiFlow.ViewModels
{
    public class ReservarCitaVM
    {
        public int EmprendimientoId { get; set; }
        public string NombreEmprendimiento { get; set; } = string.Empty;

        public List<Servicio> Servicios { get; set; } = new();
    }
}