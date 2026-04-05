using Microsoft.EntityFrameworkCore;
using ServiFlow.Models;
using System.Collections.Generic;

namespace ServiFlow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<Emprendimiento> Emprendimientos { get; set; }
        public DbSet<Disponibilidad> Disponibilidades { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<TareaKanban> TareasKanban { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}