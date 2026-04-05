using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiFlow.Data;
using ServiFlow.Models;
using ServiFlow.ViewModels;

namespace ServiFlow.Controllers
{
    public class DisponibilidadesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DisponibilidadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? emprendimientoId)
        {
            var vm = new DisponibilidadVM();

            vm.Emprendimientos = _context.Emprendimientos
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                })
                .ToList();

            vm.EmprendimientoIdSeleccionado = emprendimientoId;

            if (emprendimientoId.HasValue)
            {
                vm.NuevaDisponibilidad.EmprendimientoId = emprendimientoId.Value;

                vm.HorariosExistentes = _context.Disponibilidades
                    .Where(d => d.EmprendimientoId == emprendimientoId.Value)
                    .ToList()
                    .OrderBy(d => d.Dia)
                    .ThenBy(d => d.HoraInicio)
                    .ToList();
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Crear(DisponibilidadVM vm)
        {
            vm.Emprendimientos = _context.Emprendimientos
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                })
                .ToList();

            if (!ModelState.IsValid)
            {
                vm.HorariosExistentes = _context.Disponibilidades
                    .Where(d => d.EmprendimientoId == vm.NuevaDisponibilidad.EmprendimientoId)
                    .ToList()
                    .OrderBy(d => d.Dia)
                    .ThenBy(d => d.HoraInicio)
                    .ToList();
                if (vm.NuevaDisponibilidad.HoraFin <= vm.NuevaDisponibilidad.HoraInicio)
                {
                    ModelState.AddModelError("", "La hora final debe ser mayor que la hora inicial.");
                }

                vm.EmprendimientoIdSeleccionado = vm.NuevaDisponibilidad.EmprendimientoId;
                return View("Index", vm);
            }

            _context.Disponibilidades.Add(vm.NuevaDisponibilidad);
            _context.SaveChanges();

            return RedirectToAction("Index", new { emprendimientoId = vm.NuevaDisponibilidad.EmprendimientoId });
        }

        public IActionResult Eliminar(int id)
        {
            var disponibilidad = _context.Disponibilidades.Find(id);

            if (disponibilidad == null)
                return RedirectToAction("Index");

            int empId = disponibilidad.EmprendimientoId;

            _context.Disponibilidades.Remove(disponibilidad);
            _context.SaveChanges();

            return RedirectToAction("Index", new { emprendimientoId = empId });
        }
    }
}
