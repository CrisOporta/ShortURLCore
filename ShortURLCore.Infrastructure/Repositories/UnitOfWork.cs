using ShortURLCore.Infrastructure.Data;
using ShortURLCore.Infrastructure.Repositories.IRepositories;
using ShortURLCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortURLCore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly ApplicationDbContext _db;
        private readonly Dictionary<Type, object> _repositories = new();
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // System
        private IRepository<AppConfig>? _appConfig;
        private IRepository<UrlMapping>? _urlMapping;
        public IRepository<AppConfig> AppConfigs => _appConfig ??= GetRepository<AppConfig>();
        public IRepository<UrlMapping> UrlMappings => _urlMapping ??= GetRepository<UrlMapping>();

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IRepository<T>)_repositories[typeof(T)];

            var repository = new Repository<T>(_db);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            if (!_db.ChangeTracker.HasChanges()) return 0;
            return await _db.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _db.DisposeAsync();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
