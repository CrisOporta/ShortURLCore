using Microsoft.AspNetCore.Mvc;
using ShortURLCore.Web.Services;
using ShortURLCore.Web.Services.IServices;

namespace ShortURLCore.Web.Areas.Short.Controllers
{
    [Area("Short")]
    public class SetupController : Controller
    {
        private readonly ISetupService _setupService;

        public SetupController(ISetupService setupService) {
            _setupService = setupService;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string host, string port, string dbName, string user, string pass)
        {
            string connStr = $"Host={host};Port={port};Database={dbName};Username={user};Password={pass}";

            if (!await _setupService.TestDatabaseConnection(connStr))
            {
                ModelState.AddModelError("", "No se pudo conectar a la base de datos");
                return View();
            }

            await _setupService.RunSetup(connStr);
            return RedirectToAction("Index", "UrlMappings");
        }

    }
}
