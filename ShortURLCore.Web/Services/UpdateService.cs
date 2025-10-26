using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ShortURLCore.Infrastructure.Data;
using ShortURLCore.Web.Services.IServices;

namespace ShortURLCore.Web.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IServiceProvider _provider;
        private readonly IWebHostEnvironment _env;

        public UpdateService(IServiceProvider provider, IWebHostEnvironment env)
        {
            _provider = provider;
            _env = env;
        }

        private ApplicationDbContext CreateDbContext()
        {
            var scope = _provider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private bool IsConnectionStringConfigured()
        {
            try
            {
                using var db = CreateDbContext();
                var connStr = db.Database.GetDbConnection().ConnectionString;
                return !string.IsNullOrWhiteSpace(connStr);
            }
            catch
            {
                return false;
            }
        }

        public async Task<IReadOnlyList<string>> GetAppliedMigrations()
        {
            if (!IsConnectionStringConfigured())
                return Array.Empty<string>();
            await using var db = CreateDbContext();
            var list = db.Database.GetAppliedMigrations().ToList();
            return await Task.FromResult(list);
        }

        public async Task<IReadOnlyList<string>> GetPendingMigrations()
        {
            if (!IsConnectionStringConfigured())
                return Array.Empty<string>();
            await using var db = CreateDbContext();
            var list = db.Database.GetPendingMigrations().ToList();
            return await Task.FromResult(list);
        }

        public async Task ApplyMigrations()
        {
            if (!IsConnectionStringConfigured())
                return;
            await using var db = CreateDbContext();
            await db.Database.MigrateAsync();
        }

        public async Task<(bool ok, string output, string? error)> CreateMigration(string name)
        {
            // Solo permitir en desarrollo para evitar riesgos en producción
            if (!_env.IsDevelopment())
            {
                return (false, string.Empty, "CreateMigration solo está permitido en Development");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, string.Empty, "El nombre de la migración es requerido");
            }

            // Intentar invocar 'dotnet ef migrations add <name>' desde el Web project como startup
            // Se requiere SDK y dotnet-ef instalado en el ambiente
            try
            {
                var contentRoot = _env.ContentRootPath; // ShortURLCore.Web
                var infraProject = Path.GetFullPath(Path.Combine(contentRoot, "..", "ShortURLCore.Infrastructure"));
                var args = $"ef migrations add {name} --project \"{infraProject}\" --startup-project \"{contentRoot}\" --output-dir Data/Migrations";

                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = args,
                    WorkingDirectory = contentRoot,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi)!;
                var stdOut = await proc.StandardOutput.ReadToEndAsync();
                var stdErr = await proc.StandardError.ReadToEndAsync();
                await proc.WaitForExitAsync();

                var success = proc.ExitCode == 0;
                return (success, stdOut, success ? null : stdErr);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, ex.Message);
            }
        }
    }
}
