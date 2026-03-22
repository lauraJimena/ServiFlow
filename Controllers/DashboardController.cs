using Microsoft.AspNetCore.Mvc;
using ServiFlow.Data;
namespace ServiFlow.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Citas = _context.Citas
                .OrderBy(c => c.Fecha)
                .Take(10)
                .ToList();

            ViewBag.Tareas = _context.Tareas
                .OrderBy(t => t.Completada)
                .ToList();

            return View();
        }
    }
}