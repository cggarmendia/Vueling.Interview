using System;
using Vueling.Domain.Entities;
using Vueling.Domain.Repository;

namespace Vueling.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        IRepository<T> GetRepository<T>()
            where T : class, IEntity;

        void Rollback();
    }
}
