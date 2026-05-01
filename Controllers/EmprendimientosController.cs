using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiFlow.Data;
using ServiFlow.Models;
using ServiFlow.ViewModels;
using System;
using System.IO;
using System.Linq;

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

        public IActionResult MisServicios()
        {
            var misServicios = _context.Emprendimientos
                .Where(e => e.EsPropio)
                .OrderBy(e => e.Id)
                .ToList();

            return View(misServicios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Emprendimiento emprendimiento, IFormFile? ImagenArchivo)
        {
            if (!ModelState.IsValid)
                return View(emprendimiento);

            emprendimiento.EsPropio = true;

            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
                emprendimiento.ImagenUrl = GuardarImagen(ImagenArchivo);
            else
                emprendimiento.ImagenUrl = "/images/default.png";

            _context.Emprendimientos.Add(emprendimiento);
            _context.SaveChanges();

            TempData["Mensaje"] = "Emprendimiento creado con éxito. Ahora personaliza tu página.";

            return RedirectToAction("Personalizar", new { id = emprendimiento.Id });
        }

        [HttpGet]
        public IActionResult Personalizar(int id)
        {
            var emprendimiento = _context.Emprendimientos
                .Include(e => e.Servicios)
                .FirstOrDefault(e => e.Id == id);

            if (emprendimiento == null)
                return NotFound();

            var vm = CrearPersonalizarVM(emprendimiento);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Personalizar(PersonalizarEmprendimientoVM vm)
        {
            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == vm.Id);

            if (emprendimiento == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                RecargarDatosPersonalizarVM(vm, emprendimiento);
                return View(vm);
            }

            emprendimiento.Nombre = vm.Nombre.Trim();
            emprendimiento.TipoServicio = vm.TipoServicio?.Trim();
            emprendimiento.Descripcion = vm.Descripcion?.Trim();

            if (vm.LogoArchivo != null && vm.LogoArchivo.Length > 0)
                emprendimiento.LogoUrl = GuardarImagen(vm.LogoArchivo);

            if (vm.BannerArchivo != null && vm.BannerArchivo.Length > 0)
                emprendimiento.BannerUrl = GuardarImagen(vm.BannerArchivo);

            _context.SaveChanges();

            TempData["Mensaje"] = "Personalización guardada correctamente.";

            return RedirectToAction("Personalizar", new { id = vm.Id });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var emprendimiento = _context.Emprendimientos
                .Include(e => e.Servicios)
                .FirstOrDefault(e => e.Id == id);

            if (emprendimiento == null)
                return NotFound();

            var vm = CrearPersonalizarVM(emprendimiento);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PersonalizarEmprendimientoVM vm)
        {
            if (id != vm.Id)
                return NotFound();

            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == id);

            if (emprendimiento == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                RecargarDatosPersonalizarVM(vm, emprendimiento);
                return View(vm);
            }

            emprendimiento.Nombre = vm.Nombre.Trim();
            emprendimiento.TipoServicio = vm.TipoServicio?.Trim();
            emprendimiento.Descripcion = vm.Descripcion?.Trim();

            if (vm.LogoArchivo != null && vm.LogoArchivo.Length > 0)
                emprendimiento.LogoUrl = GuardarImagen(vm.LogoArchivo);

            if (vm.BannerArchivo != null && vm.BannerArchivo.Length > 0)
                emprendimiento.BannerUrl = GuardarImagen(vm.BannerArchivo);

            _context.SaveChanges();

            TempData["Mensaje"] = "Personalización actualizada correctamente.";

            return RedirectToAction("Edit", new { id = vm.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarServicio(PersonalizarEmprendimientoVM vm)
        {
            bool esAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == vm.Id);

            if (emprendimiento == null)
            {
                if (esAjax)
                    return NotFound(new { success = false, message = "No se encontró el emprendimiento." });

                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(vm.NuevoServicioNombre))
            {
                if (esAjax)
                    return BadRequest(new { success = false, message = "Debes escribir el nombre del servicio." });

                TempData["Error"] = "Debes escribir el nombre del servicio.";
                return RedirectToAction("Personalizar", new { id = vm.Id });
            }

            string nombreServicio = vm.NuevoServicioNombre.Trim();

            bool existeServicio = _context.Servicios.Any(s =>
                s.EmprendimientoId == vm.Id &&
                s.Activo &&
                s.Nombre.ToLower() == nombreServicio.ToLower());

            if (existeServicio)
            {
                if (esAjax)
                    return BadRequest(new { success = false, message = "Ya existe un servicio con ese nombre." });

                TempData["Error"] = "Ya existe un servicio con ese nombre.";
                return RedirectToAction("Personalizar", new { id = vm.Id });
            }

            string? imagenServicioUrl = null;

            if (vm.NuevoServicioImagenArchivo != null && vm.NuevoServicioImagenArchivo.Length > 0)
                imagenServicioUrl = GuardarImagen(vm.NuevoServicioImagenArchivo);

            var servicio = new Servicio
            {
                Nombre = nombreServicio,
                Descripcion = vm.NuevoServicioDescripcion?.Trim(),
                Precio = vm.NuevoServicioPrecio,
                ImagenUrl = imagenServicioUrl,
                Activo = true,
                EmprendimientoId = vm.Id
            };

            _context.Servicios.Add(servicio);
            _context.SaveChanges();

            if (esAjax)
            {
                return Json(new
                {
                    success = true,
                    message = "Servicio agregado correctamente.",
                    servicio = new
                    {
                        id = servicio.Id,
                        emprendimientoId = servicio.EmprendimientoId,
                        nombre = servicio.Nombre,
                        descripcion = string.IsNullOrWhiteSpace(servicio.Descripcion)
                            ? "Sin descripción"
                            : servicio.Descripcion,
                        precio = servicio.Precio.HasValue
                            ? $"$ {servicio.Precio.Value:N0}"
                            : "Sin precio",
                        imagenUrl = servicio.ImagenUrl
                    }
                });
            }

            TempData["Mensaje"] = "Servicio agregado correctamente.";
            return RedirectToAction("Personalizar", new { id = vm.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DesactivarServicio(int servicioId, int emprendimientoId, string? returnTo = null)
        {
            var servicio = _context.Servicios
                .FirstOrDefault(s => s.Id == servicioId && s.EmprendimientoId == emprendimientoId);

            if (servicio == null)
                return NotFound();

            servicio.Activo = false;
            _context.SaveChanges();

            TempData["Mensaje"] = "Servicio eliminado correctamente.";

            if (returnTo == "Edit")
                return RedirectToAction("Edit", new { id = emprendimientoId });

            return RedirectToAction("Personalizar", new { id = emprendimientoId });
        }

        public IActionResult Delete(int id)
        {
            var emprendimiento = _context.Emprendimientos.Find(id);

            if (emprendimiento == null)
                return NotFound();

            return View(emprendimiento);
        }

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

        [HttpGet]
        public IActionResult RankingClientes(
            int emprendimientoId,
            int page = 1,
            string? searchTerm = null,
            string searchBy = "nombre")
        {
            const int pageSize = 5;

            var emprendimiento = _context.Emprendimientos
                .FirstOrDefault(e => e.Id == emprendimientoId);

            if (emprendimiento == null)
                return NotFound();

            searchBy = string.IsNullOrWhiteSpace(searchBy)
                ? "nombre"
                : searchBy.Trim().ToLower();

            if (searchBy != "nombre" && searchBy != "correo" && searchBy != "telefono")
                searchBy = "nombre";

            var rankingBase = _context.Citas
                .Include(c => c.Usuario)
                .Where(c => c.EmprendimientoId == emprendimientoId)
                .AsEnumerable()
                .GroupBy(c => new
                {
                    c.UsuarioId,
                    Nombre = c.Usuario != null ? c.Usuario.Nombre : "Cliente",
                    Email = c.Usuario != null ? c.Usuario.Email : "",
                    Telefono = c.Usuario != null ? c.Usuario.Telefono : null
                })
                .Select(g => new RankingClienteItemVM
                {
                    UsuarioId = g.Key.UsuarioId,
                    NombreCliente = g.Key.Nombre,
                    EmailCliente = g.Key.Email,
                    TelefonoCliente = g.Key.Telefono,
                    TotalCitas = g.Count(),
                    ProximaCita = g
                        .Where(x => x.Fecha >= DateTime.Now)
                        .OrderBy(x => x.Fecha)
                        .Select(x => (DateTime?)x.Fecha)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.TotalCitas)
                .ThenBy(x => x.NombreCliente)
                .ToList();

            var rankingCompleto = rankingBase
                .Select((cliente, index) =>
                {
                    cliente.PosicionRanking = index + 1;
                    return cliente;
                })
                .ToList();

            var top3 = rankingCompleto.Take(3).ToList();

            var rankingFiltrado = rankingCompleto;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();

                rankingFiltrado = searchBy switch
                {
                    "correo" => rankingCompleto
                        .Where(x => !string.IsNullOrWhiteSpace(x.EmailCliente) &&
                                    x.EmailCliente.ToLower().Contains(term))
                        .ToList(),

                    "telefono" => rankingCompleto
                        .Where(x =>
                        {
                            if (string.IsNullOrWhiteSpace(x.TelefonoCliente))
                                return false;

                            var telefonoCliente = new string(x.TelefonoCliente.Where(char.IsDigit).ToArray());
                            var telefonoBuscado = new string(term.Where(char.IsDigit).ToArray());

                            return !string.IsNullOrWhiteSpace(telefonoBuscado) &&
                                   telefonoCliente.Contains(telefonoBuscado);
                        })
                        .ToList(),

                    _ => rankingCompleto
                        .Where(x => !string.IsNullOrWhiteSpace(x.NombreCliente) &&
                                    x.NombreCliente.ToLower().Contains(term))
                        .ToList()
                };
            }

            int totalItems = rankingFiltrado.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages <= 0)
                totalPages = 1;

            if (page < 1)
                page = 1;

            if (page > totalPages)
                page = totalPages;

            var rankingPaginado = rankingFiltrado
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new RankingClientesVM
            {
                EmprendimientoId = emprendimientoId,
                NombreEmprendimiento = emprendimiento.Nombre,
                ClientesTop = top3,
                ClientesPaginados = rankingPaginado,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SearchBy = searchBy
            };

            return View("~/Views/Emprendimientos/RankingClientes.cshtml", vm);
        }

        private PersonalizarEmprendimientoVM CrearPersonalizarVM(Emprendimiento emprendimiento)
        {
            return new PersonalizarEmprendimientoVM
            {
                Id = emprendimiento.Id,
                Nombre = emprendimiento.Nombre,
                TipoServicio = emprendimiento.TipoServicio,
                Descripcion = emprendimiento.Descripcion,
                LogoActualUrl = emprendimiento.LogoUrl,
                BannerActualUrl = emprendimiento.BannerUrl,
                Servicios = emprendimiento.Servicios
                    .Where(s => s.Activo)
                    .OrderBy(s => s.Nombre)
                    .ToList()
            };
        }

        private void RecargarDatosPersonalizarVM(PersonalizarEmprendimientoVM vm, Emprendimiento emprendimiento)
        {
            vm.LogoActualUrl = emprendimiento.LogoUrl;
            vm.BannerActualUrl = emprendimiento.BannerUrl;

            vm.Servicios = _context.Servicios
                .Where(s => s.EmprendimientoId == vm.Id && s.Activo)
                .OrderBy(s => s.Nombre)
                .ToList();
        }

        private string GuardarImagen(IFormFile archivo)
        {
            string carpeta = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string extension = Path.GetExtension(archivo.FileName);
            string nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            return "/images/" + nombreArchivo;
        }
    }
}