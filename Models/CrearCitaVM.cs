using System.ComponentModel.DataAnnotations;

namespace ServiFlow.ViewModels
{
    public class CrearCitaVM
    {
        [Required]
        public int EmprendimientoId { get; set; }

        [Required]
        public int ServicioId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string HoraSeleccionada { get; set; } = string.Empty;
    }
}