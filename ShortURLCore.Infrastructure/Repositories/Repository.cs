using Microsoft.EntityFrameworkCore;
using ShortURLCore.Infrastructure.Data;
using ShortURLCore.Infrastructure.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShortURLCore.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task<List<T>?> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool tracked = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool tracked = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

           
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public async Task CreateAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            var createdAtProp = typeof(T).GetProperty("CreatedAt");

            if (createdAtProp != null && createdAtProp.PropertyType == typeof(DateTime?) && createdAtProp.CanWrite)
            {
                createdAtProp.SetValue(entity, DateTime.UtcNow);
            }

            await dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            var updatedAtProp = typeof(T).GetProperty("UpdatedAt");

            if (updatedAtProp != null && updatedAtProp.PropertyType == typeof(DateTime?) && updatedAtProp.CanWrite)
            {
                updatedAtProp.SetValue(entity, DateTime.UtcNow);
            }

            dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}
