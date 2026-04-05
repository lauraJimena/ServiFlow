using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiFlow.Data;
using ServiFlow.Models;
using ServiFlow.ViewModels;

namespace ServiFlow.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UsuarioEmail") != null)
            {
                return RedirigirSegunRol();
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View(model);
            }

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario);

            return RedirigirSegunTipo(usuario.TipoUsuario);
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(RegistroVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existe = _context.Usuarios.Any(u => u.Email == model.Email);
            if (existe)
            {
                ModelState.AddModelError("Email", "Ese correo ya está registrado");
                return View(model);
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Password = model.Password,
                TipoUsuario = model.TipoUsuario
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario);

            return RedirigirSegunTipo(usuario.TipoUsuario);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        private IActionResult RedirigirSegunRol()
        {
            var tipo = HttpContext.Session.GetString("TipoUsuario");
            return RedirigirSegunTipo(tipo);
        }

        private IActionResult RedirigirSegunTipo(string? tipo)
        {
            if (tipo == "Emprendedor")
                return RedirectToAction("InicioEmprendedor", "Emprendimientos");

            return RedirectToAction("InicioCliente", "Cliente");
        }
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear(); // 🔥 borra TODA la sesión
            return RedirectToAction("Login", "Usuarios");
        }
    }
}
