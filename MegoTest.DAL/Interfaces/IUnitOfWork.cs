using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MegoTest.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IBaseRepository<T> GetRepository<T>() where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
