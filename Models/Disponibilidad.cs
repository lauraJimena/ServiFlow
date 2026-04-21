using System.ComponentModel.DataAnnotations.Schema;

namespace ServiFlow.Models
{
    [Table("Disponibilidades")]
    public class Disponibilidad
    {
        public int Id { get; set; }

        public int EmprendimientoId { get; set; }
        public Emprendimiento? Emprendimiento { get; set; }

        public int ServicioId { get; set; }
        public Servicio? Servicio { get; set; }

        public DayOfWeek Dia { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}