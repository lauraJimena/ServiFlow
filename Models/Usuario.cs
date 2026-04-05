namespace ServiFlow.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // "Cliente" o "Emprendedor"
        public string TipoUsuario { get; set; } = string.Empty;
    }
}
