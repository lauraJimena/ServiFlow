using System.ComponentModel.DataAnnotations;

namespace ServiFlow.Models
{
    public class Cita
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int EmprendimientoId { get; set; }
        public Emprendimiento? Emprendimiento { get; set; }

        public int ServicioId { get; set; }
        public Servicio? Servicio { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";
    }
}