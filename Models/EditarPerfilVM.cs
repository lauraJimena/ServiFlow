using System.ComponentModel.DataAnnotations;

namespace ServiFlow.ViewModels
{
    public class EditarPerfilVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public bool CambiarPassword { get; set; }

        public string? PasswordActual { get; set; }

        public string? NuevaPassword { get; set; }

        public string? ConfirmarNuevaPassword { get; set; }
    }
}