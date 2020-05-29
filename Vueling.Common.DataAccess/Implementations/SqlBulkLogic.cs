using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vueling.Common.Core.Log;
using Vueling.Common.DataAccess.Contracts;
using Vueling.Domain.Entities;

namespace Vueling.Common.DataAccess.Implementations
{
    public class SqlBulkLogic : ISqlBulkLogic
    {
        #region Public_Methods
        public void BulkDelete(string tableName)
        {
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VuelingContext"].ConnectionString))
            {
                sqlConnection.Open();

                using (SqlTransaction sqlTran = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        string deleteQuery = $"Delete from dbo.{tableName}";
                        SqlCommand sqlComm = new SqlCommand(deleteQuery, sqlConnection, sqlTran);
                        sqlComm.ExecuteNonQuery();
                        sqlTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        sqlTran.Rollback();
                        Logger.AddLOGMsg($"Failed SqlBulkDelete method. Message: {ex.Message}. Exception: {ex}.");
                    }
                }

                sqlConnection.Close();
            }
        }

        public void BulkInsert<T>(IEnumerable<T> entities, string tableName) where T : class, IEntity
        {
            var table = GetDataTable(entities, tableName);

            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VuelingContext"].ConnectionString))
            {
                sqlConnection.Open();
                using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                {
                    using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
                    {
                        bulkCopy.DestinationTableName = $"dbo.{tableName}";
                        AddColumnMappings(typeof(T).GetProperties().Select(prop => prop.Name), bulkCopy);
                        try
                        {
                            bulkCopy.WriteToServer(table);
                            sqlTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            sqlTransaction.Rollback();
                        }
                    }
                }

                sqlConnection.Close();
            }
        }
        #endregion

        #region Private_Methods
        private DataTable GetDataTable<T>(IEnumerable<T> entities, string tableName) where T : class, IEntity
        {
            var table = CreateDataTable(tableName);

            AddColumns(entities.FirstOrDefault(), table);

            entities.ToList().ForEach(entity => AddRow(entity, table));

            return table;
        }

        private void AddColumnMappings(IEnumerable<string> fixColumnMappings, SqlBulkCopy bulkCopy)
        {
            if (fixColumnMappings.Any())
            {
                fixColumnMappings.ToList().ForEach(columnMappings => bulkCopy.ColumnMappings.Add(columnMappings, columnMappings));
            }
        }

        private DataTable CreateDataTable(string tableName)
        {
            return new DataTable()
            {
                TableName = tableName
            };
        }

        private void AddRow<T>(T entity, DataTable table) where T : class, IEntity
        {
            var row = table.NewRow();
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
                row[property.Name] = property.GetValue(entity, null);
            table.Rows.Add(row);
        }

        private void AddColumns<T>(T entity, DataTable table)
            where T : class, IEntity
        {
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                table.Columns.Add(new DataColumn(name, property.PropertyType)
                {
                    AllowDBNull = name.StartsWith("Is") && char.IsUpper(name[2]) && property.PropertyType.Equals(typeof(bool))
                });
            }
        }
        #endregion
    }
}
