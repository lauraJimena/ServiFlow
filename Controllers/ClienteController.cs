using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiFlow.Data;
using ServiFlow.Models;

namespace ServiFlow.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult InicioCliente(int page = 1)
        {
            int pageSize = 8;

            var query = _context.Emprendimientos
                .Where(e => e.EsPropio);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var emprendimientos = query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(emprendimientos);
        }


        public IActionResult Detalle(int id)
        {
            var emprendimiento = _context.Emprendimientos
                .Include(e => e.Servicios)
                .Include(e => e.Usuario)
                .FirstOrDefault(e => e.Id == id && e.EsPropio);

            if (emprendimiento == null)
                return NotFound();

            int? usuarioId = ObtenerUsuarioId();

            var calificaciones = _context.Calificaciones
                .Where(c => c.EmprendimientoId == id);

            var vm = new ClienteEmprendimientoVM
            {
                EmprendimientoId = emprendimiento.Id,
                Nombre = emprendimiento.Nombre,
                TipoServicio = emprendimiento.TipoServicio,
                Descripcion = emprendimiento.Descripcion,
                LogoUrl = emprendimiento.LogoUrl,
                BannerUrl = emprendimiento.BannerUrl,
                Servicios = emprendimiento.Servicios
                    .Where(s => s.Activo)
                    .OrderBy(s => s.Nombre)
                    .ToList(),
                PromedioCalificacion = calificaciones.Any()
                    ? calificaciones.Average(c => c.Valor)
                    : 0,
                TotalCalificaciones = calificaciones.Count(),
                MiCalificacion = usuarioId == null
                    ? null
                    : calificaciones
                        .Where(c => c.UsuarioId == usuarioId.Value)
                        .Select(c => (int?)c.Valor)
                        .FirstOrDefault(),
                NombreEmprendedor = emprendimiento.Usuario.Nombre,            
                TelefonoContacto = emprendimiento.Usuario.Telefono
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calificar(int emprendimientoId, int valor)
        {
            int? usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
            {
                TempData["Error"] = "Debes iniciar sesión para calificar.";
                return RedirectToAction("Detalle", new { id = emprendimientoId });
            }

            if (valor < 1 || valor > 5)
            {
                TempData["Error"] = "La calificación debe estar entre 1 y 5.";
                return RedirectToAction("Detalle", new { id = emprendimientoId });
            }

            var existente = _context.Calificaciones
                .FirstOrDefault(c =>
                    c.EmprendimientoId == emprendimientoId &&
                    c.UsuarioId == usuarioId.Value);

            if (existente == null)
            {
                _context.Calificaciones.Add(new Calificacion
                {
                    EmprendimientoId = emprendimientoId,
                    UsuarioId = usuarioId.Value,
                    Valor = valor,
                    Fecha = DateTime.Now
                });
            }
            else
            {
                existente.Valor = valor;
                existente.Fecha = DateTime.Now;
            }

            _context.SaveChanges();

            TempData["Mensaje"] = "Gracias por calificar este emprendimiento.";

            return RedirectToAction("Detalle", new { id = emprendimientoId });
        }

        private int? ObtenerUsuarioId()
        {
            var usuarioIdTexto = HttpContext.Session.GetString("UsuarioId");

            if (int.TryParse(usuarioIdTexto, out int usuarioId))
                return usuarioId;

            return null;
        }
    }
}