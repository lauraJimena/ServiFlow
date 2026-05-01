using System.ComponentModel.DataAnnotations;

namespace ServiFlow.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public decimal? Precio { get; set; }

        public string? ImagenUrl { get; set; }

        public bool Activo { get; set; } = true;

        public int EmprendimientoId { get; set; }

        public Emprendimiento? Emprendimiento { get; set; }

        public List<Disponibilidad> Disponibilidades { get; set; } = new();

        public List<Cita> Citas { get; set; } = new();
    }
}