namespace ShortURLCore.Web.Services.IServices
{
    public interface ISetupService
    {
        Task<bool> TestDatabaseConnection(string connString);
        Task RunSetup(string connString);
    }
}
