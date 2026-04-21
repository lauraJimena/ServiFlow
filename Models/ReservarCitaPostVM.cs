using System.ComponentModel.DataAnnotations;

namespace ServiFlow.Models
{
    public class ReservarCitaPostVM
    {
        [Required]
        public int EmprendimientoId { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El servicio es obligatorio.")]
        public string Servicio { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una hora.")]
        public string HoraSeleccionada { get; set; } = string.Empty;
    }
}
