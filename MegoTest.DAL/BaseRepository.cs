using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MegoTest.DAL.Interfaces;
using MegoTest.DAL.Entities;
using MegoTest.DAL.Models;
using MegoTest.Common.Extensions;

namespace MegoTest.DAL
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected DbContext _context { get; private set; }
        public BaseRepository(DbContext dbContext) => _context = dbContext;

        public IQueryable<TEntity> All() => _context.Set<TEntity>();

        public IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = All();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<TEntity> Get(out int total, string orderBy, int skip, int take = 0,
            bool descOrder = true, IEnumerable<Expression<Func<TEntity, bool>>> conditions = null)
        {
            var query = All();

            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    query = query.Where(condition);
                }
            }

            query = query.OrderByField(orderBy, descOrder);
            total = query.Count();
            query = query.Skip(skip);
            if (take != 0)
            {
                query = query.Take(take);
            }
            return query;
        }

        public TEntity GetById(Guid id) => _context.Find<TEntity>(id);
        public ValueTask<TEntity> GetByIdAsync(Guid id) => _context.FindAsync<TEntity>(id);
        public bool Exists(Guid id) => _context.Find<TEntity>(id) != null;

        public ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity) => _context.AddAsync(entity);
        public Task InsertAsync(IEnumerable<TEntity> entities) => _context.AddRangeAsync(entities);
        public void Insert(TEntity entity) => _context.Add(entity);
        public void Insert(IEnumerable<TEntity> entities) => _context.AddRange(entities);

        public void Update(TEntity entity) => _context.Update(entity);
        public void Update(IEnumerable<TEntity> entities) => _context.UpdateRange(entities);

        public void Delete(TEntity entity) => _context.Remove(entity);
        public void Delete(IEnumerable<TEntity> entities) => _context.RemoveRange(entities);
        public void Delete(params object[] id) => _context.Remove(_context.Find<TEntity>(id));
    }
}
