using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vueling.Domain.Entities;

namespace Vueling.Domain.Repository
{
    public interface IRepository<TEntity>
           where TEntity : class, IEntity
    {
        IQueryable<TEntity> AsQueryable();

        void Delete(TEntity entityToDelete);

        void DeleteAll();

        IEnumerable<TEntity> GetByFilters(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        List<TEntity> GetAll();

        TEntity GetByPKs(params object[] keyValues);

        void Add(TEntity entity);

        void Update(TEntity entity);
    }
}
