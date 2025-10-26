using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShortURLCore.Infrastructure.Data;
using ShortURLCore.Models;
using ShortURLCore.Web.Services.IServices;

namespace ShortURLCore.Web.Services
{
    public class SetupService : ISetupService
    {
        private readonly IServiceProvider _provider;

        public SetupService(IServiceProvider provider) => _provider = provider;

        public async Task<bool> TestDatabaseConnection(string connString)
        {
            try
            {
                await using var conn = new Npgsql.NpgsqlConnection(connString);
                await conn.OpenAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task RunSetup(string connString)
        {
            var json = File.ReadAllText("appsettings.json");
            dynamic config = Newtonsoft.Json.JsonConvert.DeserializeObject(json)!;
            config.ConnectionStrings.DefaultConnection = connString;
            File.WriteAllText("appsettings.json", Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));

            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connString).Options;
            await using var db = new ApplicationDbContext(dbOptions);
            var hasMigrations = db.Database.GetMigrations().Any();

            if (!hasMigrations)
                await db.Database.EnsureCreatedAsync();
            else
                await db.Database.MigrateAsync();

            // Evitar duplicar configuración si ya existe un registro
            var existing = await db.AppConfigs.FirstOrDefaultAsync();
            if (existing is null)
            {
                db.AppConfigs.Add(new AppConfig { DbConnectionString = connString, IsInstalled = true, CreatedAt = DateTime.Now });
            }
            else
            {
                existing.DbConnectionString = connString;
                existing.IsInstalled = true;
                existing.UpdatedAt = DateTime.Now;
                db.AppConfigs.Update(existing);
            }
            await db.SaveChangesAsync();

            File.WriteAllText("install.lock", DateTime.UtcNow.ToString());
        }
    }
}
