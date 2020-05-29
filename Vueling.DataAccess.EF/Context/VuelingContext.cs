using System;
using System.Data.Entity.ModelConfiguration;
using Vueling.Common.DataAccess.EF.Context;
using Vueling.Domain.Entities.Modules.Executives;

namespace Vueling.DataAccess.EF.Context
{
    using System.Data.Entity;
    using System.Reflection;
    using System.Linq;
    using Common.DataAccess.EF.Configuration;

    public class VuelingContext : DBContextBase
    {
        public VuelingContext()
            : base("name=VuelingContext")
        {

        }

        #region Executives
        public virtual DbSet<Rates> Rates { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Carga todas las EntityTypeConfiguration por reflection.
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                               type != typeof(DbContextBaseConfiguration<>) &&
                               (type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>) ||
                                type.BaseType.GetGenericTypeDefinition() == typeof(DbContextBaseConfiguration<>) ) );

            foreach (var configurationInstance in typesToRegister.Select(Activator.CreateInstance))
            {
                modelBuilder.Configurations.Add((dynamic)configurationInstance);
            }

        }
    }
}