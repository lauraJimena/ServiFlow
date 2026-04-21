using System.ComponentModel.DataAnnotations;

namespace ServiFlow.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public int EmprendimientoId { get; set; }
        public Emprendimiento? Emprendimiento { get; set; }

        public bool Activo { get; set; } = true;
    }
}