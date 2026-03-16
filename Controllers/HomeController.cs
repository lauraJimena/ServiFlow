using ServiFlow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        int pageSize = 10;

        var query = _context.Emprendimientos
            .AsNoTracking()
            .OrderBy(e => e.Id);

        int totalItems = query.Count();

        var emprendimientos = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View(emprendimientos);
    }
}