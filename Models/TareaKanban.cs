using System.ComponentModel.DataAnnotations;

namespace ServiFlow.Models
{
    public class TareaKanban
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public EstadoTarea Estado { get; set; }

        public int Orden { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public int EmprendimientoId { get; set; }
        public Emprendimiento? Emprendimiento { get; set; }
    }
}
