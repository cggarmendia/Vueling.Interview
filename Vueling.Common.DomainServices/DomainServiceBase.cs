using Vueling.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vueling.Common.Core.Context;
using Vueling.Common.Core.DomainServices;
using Vueling.Common.Core.IoC;
using Vueling.Common.Core.Key;
using Vueling.Domain.Entities;
using Vueling.Domain.UnitOfWork;
using Vueling.DataAccess.EF.Context;

namespace Vueling.Common.DomainServices
{
    public abstract class DomainServiceBase<TEntity> : IDomainServiceBase<TEntity>
       where TEntity : class, IEntity
    {
        #region Properties
        protected IRepository<TEntity> repository
        {
            get
            {
                ICallContext callContext = DependencyManager.Instance().Resolve<ICallContext>();

                if (!callContext.Contains(AppKeyConst.UoW) || callContext.Retrieve<IUnitOfWork>(AppKeyConst.UoW) == null)
                    throw new Exception("Unit of Work not generated or already closed");

                IUnitOfWork uow = callContext.Retrieve<IUnitOfWork>(AppKeyConst.UoW);

                return uow.GetRepository<TEntity>();
            }
        }
        #endregion Properties

        #region Public_Methods

        public void Delete(TEntity entity)
        {
            OnDelete(entity);

            repository.Delete(entity);
        }

        public void Add(TEntity entity)
        {
            // Extensibility Point
            OnAdd(entity);

            repository.Add(entity);
        }

        public void Update(TEntity entity)
        {
            // Extensibility Point
            OnUpdate(entity);

            repository.Update(entity);
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return repository.AsQueryable();
        }

        public List<TEntity> GetAll()
        {
            return repository.GetAll();
        }

        public TEntity GetByPKs(params object[] keyValues)
        {
            return repository.GetByPKs(keyValues);
        }

        public virtual IEnumerable<TEntity> GetByFilters(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
        {
            return repository.GetByFilters(filter, orderBy, includeProperties);
        }

        public VuelingContext GetNewContext()
        {
            return new VuelingContext();
        }

        #endregion Methods

        #region Protected_Methods

        protected virtual void OnDelete(TEntity entity)
        {
        }

        protected virtual void OnAdd(TEntity entity)
        {
        }

        protected virtual void OnUpdate(TEntity entity)
        {
        }

        #endregion Protected Methods
    }
}
