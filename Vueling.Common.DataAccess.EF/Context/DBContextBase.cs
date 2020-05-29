namespace Vueling.Common.DataAccess.EF.Context
{
    using System.Data.Entity;

    public abstract class DBContextBase : DbContext
    {
        public DBContextBase(string connString)
            : base(connString)
        {
        }

        #region Public Methods

        public void Commit()
        {
            //ToDo: Si se quisiera hacer algo antes del SaveChange
            /*
            foreach (var entry in ChangeTracker.Entries())
            {
            }
            */
            SaveChanges();
        }

        public void Rollback()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // TODO: Si se quisiera algo en particular en el Rollback
                if (entry.State == EntityState.Unchanged)
                    continue;

                if (entry.State == EntityState.Added)
                    continue;
                entry.Reload();
            }
        }

        #endregion Public Methods
    }
}