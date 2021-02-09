using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MegoTest.DAL.Interfaces
{
    public interface IBaseRepository { }

    public interface IBaseRepository<T> : IBaseRepository where T : class
    {
        IQueryable<T> All();
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> Get(out int total, string orderBy, int skip, int take = 0,
            bool descOrder = true, IEnumerable<Expression<Func<T, bool>>> conditions = null);
        ValueTask<T> GetByIdAsync(Guid id);
        T GetById(Guid id);
        ValueTask<EntityEntry<T>> InsertAsync(T entity);
        void Insert(T entity);
        Task InsertAsync(IEnumerable<T> entities);
        void Insert(IEnumerable<T> entities);
        void Update(T entity);
        void Update(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(params object[] id);
        void Delete(IEnumerable<T> entities);
        bool Exists(Guid id);
    }
}
