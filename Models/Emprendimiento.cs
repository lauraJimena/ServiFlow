namespace ServiFlow.Models;
using System.ComponentModel.DataAnnotations;

public class Emprendimiento
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? TipoServicio { get; set; }

    public string? Descripcion { get; set; }

    public string? ImagenUrl { get; set; }

    public bool EsPropio { get; set; } = true;

    public List<TareaKanban> TareasKanban { get; set; } = new();
}
