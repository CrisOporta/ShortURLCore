using Microsoft.EntityFrameworkCore;

namespace ShortURLCore.Web.Services.IServices
{
    public interface IUpdateService
    {
        Task<IReadOnlyList<string>> GetAppliedMigrations();
        Task<IReadOnlyList<string>> GetPendingMigrations();
        Task ApplyMigrations();

        // Dev-only: attempt to create a migration by invoking the EF CLI
        Task<(bool ok, string output, string? error)> CreateMigration(string name);
    }
}
