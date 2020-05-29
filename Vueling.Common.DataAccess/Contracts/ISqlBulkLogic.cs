using System.Collections.Generic;
using Vueling.Domain.Entities;

namespace Vueling.Common.DataAccess.Contracts
{
    public interface ISqlBulkLogic
    {
        void BulkDelete(string tableName);

        void BulkInsert<T>(IEnumerable<T> entities, string tableName) where T : class, IEntity;
    }
}