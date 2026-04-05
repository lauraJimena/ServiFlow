using ServiFlow.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServiFlow.ViewModels
{
    public class DisponibilidadVM
    {
        public int? EmprendimientoIdSeleccionado { get; set; }

        public List<SelectListItem> Emprendimientos { get; set; } = new();

        public Disponibilidad NuevaDisponibilidad { get; set; } = new();

        public List<Disponibilidad> HorariosExistentes { get; set; } = new();
    }
}