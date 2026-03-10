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

        public EmprendimientosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTAR
        public IActionResult MisServicios()
        {
            var misServicios = _context.Emprendimientos
                .Where(e => e.EsPropio == true)
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
        public IActionResult Create(Emprendimiento emprendimiento, IFormFile ImagenArchivo)
        {
            if (ModelState.IsValid)
            {
                emprendimiento.EsPropio = true;
                if (ImagenArchivo != null && ImagenArchivo.Length > 0)
                {
                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ImagenArchivo.FileName);
                    var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nombreArchivo);

                    using (var stream = new FileStream(ruta, FileMode.Create))
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

            return View(emprendimiento);
        }
        public IActionResult Edit(int id)
        {
            var emprendimiento = _context.Emprendimientos.Find(id);
            if (emprendimiento == null)
                return NotFound();

            return View(emprendimiento);
        }
        [HttpPost]
        public IActionResult Edit(int id, Emprendimiento emprendimiento)
        {
            var existente = _context.Emprendimientos.Find(id);

            if (existente == null)
                return NotFound();

            existente.Nombre = emprendimiento.Nombre;
            existente.TipoServicio = emprendimiento.TipoServicio;
            existente.Descripcion = emprendimiento.Descripcion;
            existente.EsPropio = emprendimiento.EsPropio;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var emprendimiento = _context.Emprendimientos.Find(id);

            if (emprendimiento != null)
            {
                _context.Emprendimientos.Remove(emprendimiento);
                _context.SaveChanges();
            }

            return RedirectToAction("MisServicios");
        }
    }
}