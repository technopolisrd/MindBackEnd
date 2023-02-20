using Common.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Core.General;
using Common.Core.Extensions;
using System.Threading.Tasks;

namespace Common.Core.Base
{
    public abstract class RepositoryBase<TEntity, U> : IDataRepository<TEntity> where TEntity : class, new() where U : DbContext
    {

        protected readonly U _Context;
        private readonly DbSet<TEntity> _DbSet;

        public RepositoryBase(U context)
        {
            _Context = context;
            _DbSet = _Context.Set<TEntity>();
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            _Context.Set<TEntity>().Add(entity);

            await _Context.SaveChangesAsync();

            return entity;
        }

        public virtual async void Remove(TEntity entity)
        {
            _DbSet.Attach(entity);
            _Context.Entry<TEntity>(entity).State = EntityState.Deleted;

            await _Context.SaveChangesAsync();
        }

        public virtual async void Remove(long id)
        {
            TEntity entity = _DbSet.Find(id);

            _Context.Entry<TEntity>(entity).State = EntityState.Deleted;
            await _Context.SaveChangesAsync();
        }

        public virtual async Task<TEntity> Update(TEntity entity)
        {
            _DbSet.Attach(entity);
            _Context.Entry<TEntity>(entity).State = EntityState.Modified;

            await _Context.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAll(string sortExpression = null)
        {
            return await _DbSet.AsNoTracking().OrderBy(sortExpression).ToListAsync();
        }

        public async Task<IPagedList<TEntity>> GetPaged(int startRowIndex, int pageSize, string sortExpression = null)
        {
            return new PagedList<TEntity>(await _DbSet.AsNoTracking().OrderBy(sortExpression).ToListAsync(), startRowIndex, pageSize);
        }

        public async Task<IEnumerable<TEntity>> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> transform, Expression<Func<TEntity, bool>> filter = null, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking().OrderBy(sortExpression) : _DbSet.AsNoTracking().Where(filter).OrderBy(sortExpression);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? notSortedResults : notSortedResults.OrderBy(sortExpression);

            return await sortedResults.ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAll<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> transform, Expression<Func<TEntity, bool>> filter = null, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking().OrderBy(sortExpression) : _DbSet.AsNoTracking().Where(filter).OrderBy(sortExpression);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? notSortedResults : notSortedResults.OrderBy(sortExpression);

            return await sortedResults.ToListAsync();
        }

        public int GetCount<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> transform, Expression<Func<TEntity, bool>> filter = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking() : _DbSet.AsNoTracking().Where(filter);

            return transform(query).Count();
        }

        public async Task<IPagedList<TEntity>> GetPaged(Func<IQueryable<TEntity>, IQueryable<TEntity>> transform, Expression<Func<TEntity, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking().OrderBy(sortExpression) : _DbSet.AsNoTracking().Where(filter).OrderBy(sortExpression);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? await notSortedResults.ToListAsync() : await notSortedResults.OrderBy(sortExpression).ToListAsync();

            return new PagedList<TEntity>(sortedResults, startRowIndex, pageSize);
        }

        public async Task<IPagedList<TResult>> GetPaged<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> transform, Expression<Func<TEntity, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking() : _DbSet.AsNoTracking().Where(filter);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? await notSortedResults.ToListAsync() : await notSortedResults.OrderBy(sortExpression).ToListAsync();

            return new PagedList<TResult>(sortedResults, startRowIndex, pageSize);
        }

        public async Task<TEntity> Get(long id)
        {
            return await _DbSet.FindAsync(id);
        }

        public async Task<TEntity> Get(Guid id)
        {
            return await _DbSet.FindAsync(id);
        }

        public async Task<TResult> Get<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> transform, Expression<Func<TEntity, bool>> filter = null, string sortExpression = null)
        {
            var query = filter == null ? _DbSet : _DbSet.Where(filter);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? await notSortedResults.ToListAsync() : await notSortedResults.OrderBy(sortExpression).ToListAsync();

            return sortedResults.FirstOrDefault();
        }
        public bool Exists(long id)
        {
            return _DbSet.Find(id) != null;
        }

        public bool Exists(Func<IQueryable<TEntity>, IQueryable<TEntity>> selector, Expression<Func<TEntity, bool>> filter = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking() : _DbSet.AsNoTracking().Where(filter);

            var result = selector(query);

            return result.Any();
        }


    }

}
