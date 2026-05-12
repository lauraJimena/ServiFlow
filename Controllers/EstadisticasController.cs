using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiFlow.Data;
using ServiFlow.Models;

namespace ServiFlow.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EstadisticasController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int emprendimientoId)
        {
            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == emprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            var totalCitas = _context.Citas
                .Count(c => c.EmprendimientoId == emprendimientoId);

            var topServicio = _context.Citas
                .Where(c => c.EmprendimientoId == emprendimientoId)
                .GroupBy(c => c.ServicioId)
                .Select(g => new
                {
                    ServicioId = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefault();

            string nombreServicio = "Sin datos";
            int citasTop = 0;

            if (topServicio != null)
            {
                var servicio = _context.Servicios
                    .FirstOrDefault(s => s.Id == topServicio.ServicioId);

                nombreServicio = servicio?.Nombre ?? "Sin datos";
                citasTop = topServicio.Total;
            }

            var vm = new EstadisticasVM
            {
                ServicioMasSolicitado = nombreServicio,
                TotalCitas = citasTop,
                TotalGeneralCitas = totalCitas,
                Id = emprendimientoId
            };

            var serviciosStats = _context.Citas
                .Where(c => c.EmprendimientoId == emprendimientoId)
                .GroupBy(c => c.Servicio.Nombre)
                .Select(g => new
                {
                    Servicio = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            ViewBag.NombreEmprendimiento = emprendimiento.Nombre;
            ViewBag.ServiciosLabels = serviciosStats.Select(s => s.Servicio).ToList();
            ViewBag.ServiciosData = serviciosStats.Select(s => s.Total).ToList();

            return View(vm);
        }


    }
}
