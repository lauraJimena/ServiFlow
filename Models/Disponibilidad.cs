namespace ServiFlow.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Disponibilidades")]
    public class Disponibilidad
    {
        public int Id { get; set; }

        public int EmprendimientoId { get; set; }
        public Emprendimiento? Emprendimiento { get; set; }

        public DayOfWeek Dia { get; set; } // Lunes, Martes, etc

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    
}
}
