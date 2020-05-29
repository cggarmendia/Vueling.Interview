using Vueling.Domain.UnitOfWork;

namespace Vueling.Common.DataAccess.EF.UnitOfWork
{
    public interface IUnitOfWorkManager
    {
        #region Public Methods

        IUnitOfWork GetUoW();

        #endregion Public Methods
    }
}
