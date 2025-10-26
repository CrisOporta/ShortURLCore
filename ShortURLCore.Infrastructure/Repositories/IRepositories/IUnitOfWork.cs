using ShortURLCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortURLCore.Infrastructure.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        IRepository<AppConfig> AppConfigs { get; }
        IRepository<UrlMapping> UrlMappings { get; }
    }
}
