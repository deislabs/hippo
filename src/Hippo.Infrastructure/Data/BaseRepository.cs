using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Infrastructure.Data
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class, IAggregateRoot
    {
        private readonly DbContext dbContext;

        public RepositoryBase(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            dbContext.Set<T>().Add(entity);
            await SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<T>().Where(filter).CountAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            dbContext.Set<T>().Remove(entity);
            await SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            dbContext.Set<T>().RemoveRange(entities);
            await SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<T> GetAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            return await dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        }

        public virtual async Task<List<T>> ListAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, CancellationToken cancellationToken = default)
        {
            return await orderBy(dbContext.Set<T>().Where(filter)).ToListAsync(cancellationToken);
        }

        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync(cancellationToken);
        }
    }
}
