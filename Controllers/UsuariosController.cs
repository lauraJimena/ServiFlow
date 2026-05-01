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
                TipoUsuario = model.TipoUsuario,
                Telefono = model.Telefono
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario);
            HttpContext.Session.SetString("UsuarioTelefono", usuario.Telefono ?? "");

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
        [HttpPost]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Usuarios");
        }

        [HttpGet]
        public IActionResult EditarPerfil()
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdString) || !int.TryParse(usuarioIdString, out int usuarioId))
                return RedirectToAction("Login", "Usuarios");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
                return RedirectToAction("Login", "Usuarios");

            var vm = new EditarPerfilVM
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Telefono = usuario.Telefono
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPerfil(EditarPerfilVM model)
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdString) || !int.TryParse(usuarioIdString, out int usuarioId))
                return RedirectToAction("Login", "Usuarios");

            if (usuarioId != model.Id)
                return RedirectToAction("Login", "Usuarios");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
                return RedirectToAction("Login", "Usuarios");

            var correoExiste = _context.Usuarios
                .Any(u => u.Email == model.Email && u.Id != usuarioId);

            if (correoExiste)
                ModelState.AddModelError("Email", "Ese correo ya está registrado por otro usuario.");

            if (model.CambiarPassword)
            {
                if (string.IsNullOrWhiteSpace(model.PasswordActual))
                    ModelState.AddModelError("PasswordActual", "Debes ingresar tu contraseña actual.");

                if (model.PasswordActual != usuario.Password)
                    ModelState.AddModelError("PasswordActual", "La contraseña actual no es correcta.");

                if (string.IsNullOrWhiteSpace(model.NuevaPassword))
                    ModelState.AddModelError("NuevaPassword", "Debes ingresar la nueva contraseña.");

                if (model.NuevaPassword != model.ConfirmarNuevaPassword)
                    ModelState.AddModelError("ConfirmarNuevaPassword", "Las contraseñas no coinciden.");
            }

            if (!ModelState.IsValid)
                return View(model);

            usuario.Nombre = model.Nombre;
            usuario.Email = model.Email;
            usuario.Telefono = model.Telefono;

            if (model.CambiarPassword)
                usuario.Password = model.NuevaPassword!;

            _context.SaveChanges();

            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioTelefono", usuario.Telefono ?? "");

            TempData["Mensaje"] = "Perfil actualizado correctamente.";

            return RedirectToAction("EditarPerfil");
        }
    }
}

