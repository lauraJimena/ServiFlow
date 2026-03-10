namespace ServiFlow.Models;
using System.ComponentModel.DataAnnotations;

public class Emprendimiento
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string TipoServicio { get; set; } // Uñas, Barbería, Pestañas

    public string Descripcion { get; set; }
    public string? ImagenUrl { get; set; }
    public bool EsPropio { get; set; }
}