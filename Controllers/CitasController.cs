using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiFlow.Data;
using ServiFlow.Models;
using ServiFlow.ViewModels;

namespace ServiFlow.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int DuracionSlotMinutos = 30;

        public CitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Reservar(int emprendimientoId)
        {
            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == emprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            var servicios = _context.Servicios
                .Where(s => s.EmprendimientoId == emprendimientoId && s.Activo)
                .OrderBy(s => s.Nombre)
                .ToList();

            var vm = new ReservarCitaVM
            {
                EmprendimientoId = emprendimiento.Id,
                NombreEmprendimiento = emprendimiento.Nombre,
                Servicios = servicios
            };

            return View("~/Views/Cliente/Reservar.cshtml", vm);
        }

        [HttpGet]
        public IActionResult ObtenerHorasDisponibles(int emprendimientoId, int servicioId, DateTime fecha)
        {
            var diaSemana = fecha.DayOfWeek;

            var disponibilidades = _context.Disponibilidades
                .Where(d => d.EmprendimientoId == emprendimientoId
                         && d.ServicioId == servicioId
                         && d.Dia == diaSemana)
                .ToList()
                .OrderBy(d => d.HoraInicio)
                .ToList();

            if (!disponibilidades.Any())
                return Json(new List<object>());

            var citasDelDia = _context.Citas
                .Where(c => c.EmprendimientoId == emprendimientoId
                         && c.ServicioId == servicioId
                         && c.Fecha.Date == fecha.Date)
                .ToList();

            var horasOcupadas = citasDelDia
                .Select(c => c.Fecha.TimeOfDay)
                .ToList();

            var horasDisponibles = new List<object>();

            foreach (var disponibilidad in disponibilidades)
            {
                var horaActual = disponibilidad.HoraInicio;

                while (horaActual < disponibilidad.HoraFin)
                {
                    bool ocupada = horasOcupadas.Any(h => h == horaActual);

                    if (!ocupada)
                    {
                        horasDisponibles.Add(new
                        {
                            value = horaActual.ToString(@"hh\:mm"),
                            text = DateTime.Today.Add(horaActual).ToString("h:mm tt")
                        });
                    }

                    horaActual = horaActual.Add(TimeSpan.FromMinutes(DuracionSlotMinutos));
                }
            }

            return Json(horasDisponibles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(CrearCitaVM vm)
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdString) || !int.TryParse(usuarioIdString, out int usuarioId))
            {
                TempData["Error"] = "Sesión expirada";
                return RedirectToAction("Login", "Usuarios");
            }

            var emprendimiento = _context.Emprendimientos.FirstOrDefault(e => e.Id == vm.EmprendimientoId);
            if (emprendimiento == null)
                return NotFound();

            var servicio = _context.Servicios.FirstOrDefault(s => s.Id == vm.ServicioId && s.EmprendimientoId == vm.EmprendimientoId);
            if (servicio == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Faltan datos para agendar.";
                return RedirectToAction("Reservar", new { emprendimientoId = vm.EmprendimientoId });
            }

            if (!TimeSpan.TryParse(vm.HoraSeleccionada, out TimeSpan hora))
            {
                TempData["Error"] = "La hora seleccionada no es válida.";
                return RedirectToAction("Reservar", new { emprendimientoId = vm.EmprendimientoId });
            }

            var fechaHoraCita = vm.Fecha.Date.Add(hora);
            var diaSemana = fechaHoraCita.DayOfWeek;
            var horaCita = fechaHoraCita.TimeOfDay;

            var disponibilidades = _context.Disponibilidades
                .Where(d => d.EmprendimientoId == vm.EmprendimientoId
                         && d.ServicioId == vm.ServicioId
                         && d.Dia == diaSemana)
                .ToList();

            bool estaDentroDeDisponibilidad = disponibilidades.Any(d =>
                horaCita >= d.HoraInicio && horaCita < d.HoraFin);

            if (!estaDentroDeDisponibilidad)
            {
                TempData["Error"] = "La hora seleccionada no está disponible.";
                return RedirectToAction("Reservar", new { emprendimientoId = vm.EmprendimientoId });
            }

            var citasExistentes = _context.Citas
                .Where(c => c.EmprendimientoId == vm.EmprendimientoId
                         && c.ServicioId == vm.ServicioId
                         && c.Fecha.Date == vm.Fecha.Date)
                .ToList();

            bool yaExiste = citasExistentes.Any(c => c.Fecha.TimeOfDay == horaCita);

            if (yaExiste)
            {
                TempData["Error"] = "Esa hora ya fue reservada.";
                return RedirectToAction("Reservar", new { emprendimientoId = vm.EmprendimientoId });
            }

            var cita = new Cita
            {
                UsuarioId = usuarioId,
                EmprendimientoId = vm.EmprendimientoId,
                ServicioId = vm.ServicioId,
                Fecha = fechaHoraCita,
                Estado = "Pendiente"
            };

            _context.Citas.Add(cita);
            _context.SaveChanges();

            TempData["Success"] = "Tu cita fue agendada correctamente.";
            return RedirectToAction("Reservar", new { emprendimientoId = vm.EmprendimientoId });
        }

        [HttpGet]
        public IActionResult MisCitas(int emprendimientoId, int page = 1)
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdString) || !int.TryParse(usuarioIdString, out int usuarioId))
            {
                TempData["Error"] = "Sesión expirada";
                return RedirectToAction("Login", "Usuarios");
            }

            const int pageSize = 3;

            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == emprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            var query = _context.Citas
                .Include(c => c.Servicio)
                .Where(c => c.UsuarioId == usuarioId &&
                            c.EmprendimientoId == emprendimientoId)
                .OrderBy(c => c.Fecha);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages == 0)
                totalPages = 1;

            if (page < 1)
                page = 1;

            if (page > totalPages)
                page = totalPages;

            var citas = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new MisCitasClienteVM
            {
                EmprendimientoId = emprendimientoId,
                NombreEmprendimiento = emprendimiento.Nombre,
                Citas = citas,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize
            };

            return View("~/Views/Cliente/MisCitas.cshtml", vm);
        }

        [HttpGet]
        public IActionResult CitasEmprendimiento(int emprendimientoId, int? servicioIdFiltro, DateTime? fechaFiltro, int page = 1)
        {
            const int pageSize = 3;

            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == emprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            var servicios = _context.Servicios
                .Where(s => s.EmprendimientoId == emprendimientoId && s.Activo)
                .OrderBy(s => s.Nombre)
                .ToList();

            var query = _context.Citas
                .Include(c => c.Usuario)
                .Include(c => c.Servicio)
                .Include(c => c.Emprendimiento)
                .Where(c => c.EmprendimientoId == emprendimientoId);

            if (servicioIdFiltro.HasValue && servicioIdFiltro.Value > 0)
            {
                query = query.Where(c => c.ServicioId == servicioIdFiltro.Value);
            }

            if (fechaFiltro.HasValue)
            {
                var fecha = fechaFiltro.Value.Date;
                query = query.Where(c => c.Fecha.Date == fecha);
            }

            var orderedQuery = query.OrderBy(c => c.Fecha);

            int totalItems = orderedQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages == 0)
                totalPages = 1;

            if (page < 1)
                page = 1;

            if (page > totalPages)
                page = totalPages;

            var citas = orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new CitasEmprendedorVM
            {
                EmprendimientoId = emprendimientoId,
                NombreEmprendimiento = emprendimiento.Nombre,
                ServicioIdFiltro = servicioIdFiltro,
                FechaFiltro = fechaFiltro,
                Servicios = servicios,
                Citas = citas,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize
            };

            return View("~/Views/Emprendimientos/CitasEmprendimiento.cshtml", vm);
        }
    }
}