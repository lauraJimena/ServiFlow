using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public IActionResult Index(int emprendimientoId, int page = 1, string tab = "configurar")
        {
            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == emprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            AsegurarServiciosBase(emprendimientoId);

            var vm = ConstruirViewModel(emprendimientoId, page, tab);
            ViewBag.NombreEmprendimiento = emprendimiento.Nombre;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(DisponibilidadVM vm)
        {
            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == vm.EmprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            AsegurarServiciosBase(vm.EmprendimientoId);

            ViewBag.NombreEmprendimiento = emprendimiento.Nombre;

            vm.TabActiva = "configurar";
            vm.Servicios = _context.Servicios
                .Where(s => s.EmprendimientoId == vm.EmprendimientoId && s.Activo)
                .OrderBy(s => s.Nombre)
                .ToList();

            var horariosQuery = _context.Disponibilidades
                .Include(d => d.Servicio)
                .Where(d => d.EmprendimientoId == vm.EmprendimientoId);

            vm.TotalItems = horariosQuery.Count();
            vm.TotalPages = (int)Math.Ceiling((double)vm.TotalItems / vm.PageSize);
            if (vm.TotalPages == 0) vm.TotalPages = 1;

            vm.HorariosExistentes = horariosQuery
                .ToList()
                .OrderBy(d => d.Dia)
                .ThenBy(d => d.HoraInicio)
                .Take(vm.PageSize)
                .ToList();

            if (vm.ServicioIdSeleccionado <= 0)
            {
                ModelState.AddModelError("", "Debes seleccionar un servicio.");
            }

            if (vm.DiaSeleccionado == null)
            {
                ModelState.AddModelError("", "Debes seleccionar un día.");
            }

            bool servicioValido = vm.Servicios.Any(s => s.Id == vm.ServicioIdSeleccionado);
            if (!servicioValido)
            {
                ModelState.AddModelError("", "El servicio seleccionado no es válido para este emprendimiento.");
            }

            if (vm.HoraFin <= vm.HoraInicio)
            {
                ModelState.AddModelError("", "La hora final debe ser mayor a la inicial.");
            }

            if (vm.DiaSeleccionado != null)
            {
                var disponibilidadesExistentes = _context.Disponibilidades
                    .Where(d => d.EmprendimientoId == vm.EmprendimientoId
                             && d.ServicioId == vm.ServicioIdSeleccionado
                             && d.Dia == vm.DiaSeleccionado.Value)
                    .ToList();

                bool cruzaHorario = disponibilidadesExistentes.Any(d =>
                    vm.HoraInicio < d.HoraFin && vm.HoraFin > d.HoraInicio);

                if (cruzaHorario)
                {
                    ModelState.AddModelError("", "Ya existe una disponibilidad que se cruza con ese rango.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Index", vm);
            }

            var disponibilidad = new Disponibilidad
            {
                EmprendimientoId = vm.EmprendimientoId,
                ServicioId = vm.ServicioIdSeleccionado,
                Dia = vm.DiaSeleccionado!.Value,
                HoraInicio = vm.HoraInicio,
                HoraFin = vm.HoraFin
            };

            _context.Disponibilidades.Add(disponibilidad);
            _context.SaveChanges();

            TempData["Success"] = "Disponibilidad creada correctamente.";
            return RedirectToAction("Index", new { emprendimientoId = vm.EmprendimientoId, tab = "horarios" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            var disponibilidad = _context.Disponibilidades
                .FirstOrDefault(d => d.Id == id);

            if (disponibilidad == null)
                return RedirectToAction("MisServicios", "Emprendimientos");

            int emprendimientoId = disponibilidad.EmprendimientoId;

            _context.Disponibilidades.Remove(disponibilidad);
            _context.SaveChanges();

            TempData["Success"] = "Disponibilidad eliminada correctamente.";
            return RedirectToAction("Index", new { emprendimientoId, tab = "horarios" });
        }

        private DisponibilidadVM ConstruirViewModel(int emprendimientoId, int page, string tab)
        {
            const int pageSize = 4;

            var servicios = _context.Servicios
                .Where(s => s.EmprendimientoId == emprendimientoId && s.Activo)
                .OrderBy(s => s.Nombre)
                .ToList();

            var horariosQuery = _context.Disponibilidades
                .Include(d => d.Servicio)
                .Where(d => d.EmprendimientoId == emprendimientoId);

            var horariosOrdenados = horariosQuery
                .ToList()
                .OrderBy(d => d.Dia)
                .ThenBy(d => d.HoraInicio)
                .ToList();

            int totalItems = horariosOrdenados.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var horariosPagina = horariosOrdenados
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new DisponibilidadVM
            {
                EmprendimientoId = emprendimientoId,
                Servicios = servicios,
                HorariosExistentes = horariosPagina,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize,
                TabActiva = string.IsNullOrWhiteSpace(tab) ? "configurar" : tab
            };
        }

        private void AsegurarServiciosBase(int emprendimientoId)
        {
            bool yaTieneServicios = _context.Servicios.Any(s => s.EmprendimientoId == emprendimientoId);

            if (yaTieneServicios)
                return;

            var serviciosBase = new List<Servicio>
            {
                new Servicio { Nombre = "Uñas", EmprendimientoId = emprendimientoId, Activo = true },
                new Servicio { Nombre = "Barbería", EmprendimientoId = emprendimientoId, Activo = true },
                new Servicio { Nombre = "Maquillaje", EmprendimientoId = emprendimientoId, Activo = true },
                new Servicio { Nombre = "Peinados", EmprendimientoId = emprendimientoId, Activo = true },
                new Servicio { Nombre = "Depilación", EmprendimientoId = emprendimientoId, Activo = true }
            };

            _context.Servicios.AddRange(serviciosBase);
            _context.SaveChanges();
        }
    }
}