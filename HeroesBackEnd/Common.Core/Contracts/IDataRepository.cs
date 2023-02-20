using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.Core.Contracts
{

    public interface IDataRepository
    {
    }

    public interface IDataRepository<T> : IDataRepository where T : class, new()
    {
        Task<T> Add(T entity);

        void Remove(T entity);

        void Remove(long id);

        Task<T> Update(T entity);

        Task<IEnumerable<T>> GetAll(string sortExpression = null);

        Task<IPagedList<T>> GetPaged(int startRowIndex, int pageSize, string sortExpression = null);

        Task<IEnumerable<T>> GetAll(Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null);

        Task<IEnumerable<TResult>> GetAll<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null);

        int GetCount<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null);

        Task<IPagedList<T>> GetPaged(Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null);

        Task<IPagedList<TResult>> GetPaged<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null);

        Task<T> Get(long id);

        Task<T> Get(Guid id);

        Task<TResult> Get<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null);

        bool Exists(long id);

        bool Exists(Func<IQueryable<T>, IQueryable<T>> query, Expression<Func<T, bool>> filter = null);
    }
}
