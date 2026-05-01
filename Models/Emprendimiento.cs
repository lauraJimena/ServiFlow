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

        // Imagen inicial / catálogo / tarjetas generales
        public string? ImagenUrl { get; set; }

        // Logo usado en Personalizar
        public string? LogoUrl { get; set; }

        // Banner usado en Personalizar
        public string? BannerUrl { get; set; }

        public bool EsPropio { get; set; } = true;

        public List<TareaKanban> TareasKanban { get; set; } = new();

        public List<Disponibilidad> Disponibilidades { get; set; } = new();

        public List<Servicio> Servicios { get; set; } = new();
    }
}