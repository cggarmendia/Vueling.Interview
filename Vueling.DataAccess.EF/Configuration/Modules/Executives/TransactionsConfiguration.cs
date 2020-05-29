using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using Vueling.Common.DataAccess.EF.Configuration;
using Vueling.Common.DataAccess.EF.Extensions;
using Vueling.Domain.Entities.Modules.Executives;

namespace Vueling.DataAccess.EF.Configuration.Modules.Executives
{
    public class TransactionsConfiguration : DbContextBaseConfiguration<Transactions>
    {
        #region Ctor.
        public TransactionsConfiguration()
            : base()
        {
            ToTable("Transactions");
            HasKey(e => e.Id);

            Property(t => t.Sku)
                .IsRequired()
                .HasMaxLength(60)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_TransactionsSku", 0) { IsUnique = false }));
            
        }
        #endregion
    }
}
