using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ServiFlow.Models
{
    public class PersonalizarEmprendimientoVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del emprendimiento es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        public string? TipoServicio { get; set; }

        public string? Descripcion { get; set; }

        public string? LogoActualUrl { get; set; }

        public string? BannerActualUrl { get; set; }

        public IFormFile? LogoArchivo { get; set; }

        public IFormFile? BannerArchivo { get; set; }

        public List<Servicio> Servicios { get; set; } = new();

        public string? NuevoServicioNombre { get; set; }

        public string? NuevoServicioDescripcion { get; set; }

        public decimal? NuevoServicioPrecio { get; set; }

        public IFormFile? NuevoServicioImagenArchivo { get; set; }
    }
}