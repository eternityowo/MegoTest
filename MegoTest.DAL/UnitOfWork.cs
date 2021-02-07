using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MegoTest.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MegoTest.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext _context;
        private readonly IDictionary<Type, IBaseRepository> _repositories;

        private readonly object _lockObject = new object();

        public UnitOfWork(DbContext context)
        {
            _repositories = new Dictionary<Type, IBaseRepository>();
            _context = context;
        }

        public IBaseRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return _repositories[typeof(T)] as IBaseRepository<T>;

            var repositoryType = typeof(BaseRepository<>).MakeGenericType(typeof(T));
            var repository = (IBaseRepository<T>)Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(typeof(T), repository);

            return repository;
        }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        public int SaveChanges()
        {
            try
            {
                Monitor.Enter(_lockObject);
                return _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Entity update failed - errors follow: " + ex.GetBaseException().Message, ex);
            }
            finally
            {
                Monitor.Exit(_lockObject);
            }
        }


        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        _context.Dispose();
                    }
                }
            }
            _disposed = true;
        }

        #endregion
    }
}
