using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShortURLCore.Infrastructure.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>?> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool tracked = false,
            CancellationToken cancellationToken = default
        );

        Task<T?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool tracked = false,
            CancellationToken cancellationToken = default
        );

        Task CreateAsync(
            T entity,
            CancellationToken cancellationToken = default
        );

        void Update(T entity);

        void Remove(T entity);
    }
}
