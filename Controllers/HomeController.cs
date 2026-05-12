using ServiFlow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiFlow.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Somos()
    {
        return View();
    }
    public IActionResult Planes()
    {
        return View();
    }
    public IActionResult Index(int page = 1)
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
    public IActionResult TestRelacion()
    {
        var data = _context.Emprendimientos
            .Include(e => e.Usuario)
            .ToList();

        var resultado = "";

        foreach (var e in data)
        {
            resultado += "==========\n";
            resultado += "Emprendimiento: " + e.Nombre + "\n";
            resultado += "UsuarioId: " + e.UsuarioId + "\n";
            resultado += "Emprendedor: " + (e.Usuario?.Nombre ?? "NULL") + "\n";
            resultado += "Teléfono: " + (e.Usuario?.Telefono ?? "NULL") + "\n\n";
        }

        if (!data.Any())
        {
            resultado = "No hay emprendimientos registrados.";
        }

        return Content(resultado, "text/plain");
    }
}