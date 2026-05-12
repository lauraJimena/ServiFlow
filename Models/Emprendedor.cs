using System.ComponentModel.DataAnnotations;
namespace ServiFlow.Models
{
    public class Emprendedor
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Ubicacion { get; set; }
        public string? HorarioAtencion { get; set; }
        public string? InstagramUrl { get; set; }
        public string? Telefono { get; set; }
    }
}
