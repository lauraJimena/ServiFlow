using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiFlow.Data;
using ServiFlow.Models;

namespace ServiFlow.Controllers
{
    public class KanbanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KanbanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int emprendimientoId)
        {
            var emprendimiento = await _context.Emprendimientos.FindAsync(emprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            ViewBag.EmprendimientoId = emprendimientoId;
            ViewBag.NombreEmprendimiento = emprendimiento.Nombre;

            var tareas = await _context.TareasKanban
                .Where(t => t.EmprendimientoId == emprendimientoId)
                .OrderBy(t => t.Estado)
                .ThenBy(t => t.Orden)
                .ToListAsync();

            return View("~/Views/Emprendimientos/Kanban.cshtml", tareas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int emprendimientoId, string titulo, string? descripcion)
        {
            int orden = await _context.TareasKanban
                .CountAsync(t => t.Estado == EstadoTarea.PorHacer && t.EmprendimientoId == emprendimientoId);

            var tarea = new TareaKanban
            {
                Titulo = titulo,
                Descripcion = descripcion,
                Estado = EstadoTarea.PorHacer,
                Orden = orden,
                EmprendimientoId = emprendimientoId
            };

            _context.TareasKanban.Add(tarea);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { emprendimientoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mover(int tareaId, EstadoTarea nuevoEstado, int emprendimientoId)
        {
            var tarea = await _context.TareasKanban.FindAsync(tareaId);

            if (tarea != null)
            {
                int nuevoOrden = await _context.TareasKanban
                    .CountAsync(t => t.Estado == nuevoEstado && t.EmprendimientoId == emprendimientoId);

                tarea.Estado = nuevoEstado;
                tarea.Orden = nuevoOrden;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { emprendimientoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int tareaId, int emprendimientoId)
        {
            var tarea = await _context.TareasKanban.FindAsync(tareaId);

            if (tarea != null)
            {
                _context.TareasKanban.Remove(tarea);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { emprendimientoId });
        }
    
    

        // GET: KanbanController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: KanbanController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KanbanController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: KanbanController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: KanbanController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: KanbanController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: KanbanController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
