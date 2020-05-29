using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Vueling.Common.DataAccess.EF.Extensions
{
    public static class TypeConfigurationExtensions
    {
        #region Public Methods

        public static PrimitivePropertyConfiguration HasUniqueIndexAnnotation(
            this PrimitivePropertyConfiguration property,
            string indexName,
            int columnOrder)
        {
            var indexAttribute = new IndexAttribute(indexName, columnOrder) { IsUnique = true };
            var indexAnnotation = new IndexAnnotation(indexAttribute);

            return property.HasColumnAnnotation(IndexAnnotation.AnnotationName, indexAnnotation);
        }

        #endregion
    }
}
