namespace ServiFlow.Models;
using System.ComponentModel.DataAnnotations;

public class Cita
{
    public int Id { get; set; }

    [Required]
    public string NombreCliente { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public string Servicio { get; set; }

    public int EmprendimientoId { get; set; }
    public Emprendimiento Emprendimiento { get; set; }
}
