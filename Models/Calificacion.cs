namespace ServiFlow.Models
{
    public class Calificacion
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int EmprendimientoId { get; set; }
        public Emprendimiento? Emprendimiento { get; set; }

        public int Valor { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}