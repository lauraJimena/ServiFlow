using System.ComponentModel.DataAnnotations;

namespace ServiFlow.Models
{
    public class Emprendimiento
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? TipoServicio { get; set; }
        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }

        public bool EsPropio { get; set; } = true;

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        
       
        

        public List<TareaKanban> TareasKanban { get; set; } = new();
        public List<Disponibilidad> Disponibilidades { get; set; } = new();
        public List<Servicio> Servicios { get; set; } = new();
        
    }
}