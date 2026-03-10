using System.ComponentModel.DataAnnotations;

public class Tarea
{
    public int Id { get; set; }

    [Required]
    public string Descripcion { get; set; }

    public bool Completada { get; set; }
}