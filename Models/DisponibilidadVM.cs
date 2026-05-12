using ServiFlow.Models;

namespace ServiFlow.ViewModels
{
    public class DisponibilidadVM
    {
        public int EmprendimientoId { get; set; }

        public List<Servicio> Servicios { get; set; } = new();

        public int ServicioIdSeleccionado { get; set; }

        public DayOfWeek? DiaSeleccionado { get; set; }
        public DateTime? FechaSeleccionada { get; set; }
        public List<DateTime> FechasSeleccionadas { get; set; } = new();

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public List<Disponibilidad> HorariosExistentes { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public int TotalItems { get; set; }

        public string TabActiva { get; set; } = "configurar";
    }
}