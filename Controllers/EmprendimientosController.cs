using Microsoft.AspNetCore.Mvc;
using ServiFlow.Data;
using ServiFlow.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ServiFlow.Controllers
{
    public class EmprendimientosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EmprendimientosController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult InicioEmprendedor(int page = 1)
        {
            int pageSize = 10;

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

        // LISTAR MIS SERVICIOS
        public IActionResult MisServicios()
        {
            var misServicios = _context.Emprendimientos
                .Where(e => e.EsPropio)
                .ToList();

            return View(misServicios);
        }

        // FORMULARIO CREAR
        public IActionResult Create()
        {
            return View();
        }

        // GUARDAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Emprendimiento emprendimiento, IFormFile? ImagenArchivo)
        {
            if (!ModelState.IsValid)
                return View(emprendimiento);

            emprendimiento.EsPropio = true;

            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                string carpetaImagenes = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(carpetaImagenes))
                    Directory.CreateDirectory(carpetaImagenes);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ImagenArchivo.FileName);
                string rutaCompleta = Path.Combine(carpetaImagenes, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    ImagenArchivo.CopyTo(stream);
                }

                emprendimiento.ImagenUrl = "/images/" + nombreArchivo;
            }

            _context.Emprendimientos.Add(emprendimiento);
            _context.SaveChanges();

            TempData["Mensaje"] = "Emprendimiento creado con éxito";
            return RedirectToAction("MisServicios");
        }

        // FORMULARIO EDITAR
        public IActionResult Edit(int id)
        {
            var emprendimiento = _context.Emprendimientos.Find(id);

            if (emprendimiento == null)
                return NotFound();

            return View(emprendimiento);
        }

        // GUARDAR EDICION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Emprendimiento emprendimiento, IFormFile? ImagenArchivo)
        {
            if (id != emprendimiento.Id)
                return NotFound();

            var existente = _context.Emprendimientos.Find(id);

            if (existente == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(emprendimiento);

            existente.Nombre = emprendimiento.Nombre;
            existente.TipoServicio = emprendimiento.TipoServicio;
            existente.Descripcion = emprendimiento.Descripcion;
            existente.EsPropio = emprendimiento.EsPropio;

            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                string carpetaImagenes = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(carpetaImagenes))
                    Directory.CreateDirectory(carpetaImagenes);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ImagenArchivo.FileName);
                string rutaCompleta = Path.Combine(carpetaImagenes, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    ImagenArchivo.CopyTo(stream);
                }

                existente.ImagenUrl = "/images/" + nombreArchivo;
            }

            _context.SaveChanges();

            TempData["Mensaje"] = "Emprendimiento actualizado con éxito";
            return RedirectToAction("MisServicios");
        }

        // CONFIRMAR ELIMINACION
        public IActionResult Delete(int id)
        {
            var emprendimiento = _context.Emprendimientos.Find(id);

            if (emprendimiento == null)
                return NotFound();

            return View(emprendimiento);
        }

        // ELIMINAR
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var emprendimiento = _context.Emprendimientos.Find(id);

            if (emprendimiento == null)
                return NotFound();

            _context.Emprendimientos.Remove(emprendimiento);
            _context.SaveChanges();

            TempData["Mensaje"] = "Emprendimiento eliminado con éxito";
            return RedirectToAction("MisServicios");
        }
    }
}