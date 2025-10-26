using Microsoft.AspNetCore.Mvc;
using ShortURLCore.Web.Services.IServices;

namespace ShortURLCore.Web.Areas.Short.Controllers
{
    [Area("Short")]
    public class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applied = await _updateService.GetAppliedMigrations();
            var pending = await _updateService.GetPendingMigrations();
            ViewBag.Applied = applied;
            ViewBag.Pending = pending;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply()
        {
            await _updateService.ApplyMigrations();
            TempData["Message"] = "Migraciones aplicadas correctamente";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMigration(string name)
        {
            var (ok, output, error) = await _updateService.CreateMigration(name);
            TempData["Message"] = ok ? $"Migración '{name}' creada" : $"Error creando migración: {error}";
            TempData["Output"] = output;
            return RedirectToAction(nameof(Index));
        }
    }
}
