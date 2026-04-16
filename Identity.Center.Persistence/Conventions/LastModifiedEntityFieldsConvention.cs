using Identity.Center.Domain.Primitives.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Identity.Center.Persistence.Conventions;

internal sealed class LastModifiedEntityFieldsConvention : IModelFinalizingConvention
{
  public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
  {
    foreach(IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes().Where(IsInherited))
    {
      IConventionProperty lastModifiedAt = entityType.FindProperty(nameof(ILastModifiedEntityFields<Guid>.LastModifiedAtUtc)) ?? throw new NullReferenceException();
      lastModifiedAt.SetColumnName("last_modified_at_utc");
      IConventionProperty lastModifiedBy = entityType.FindProperty(nameof(ILastModifiedEntityFields<Guid>.LastModifiedBy)) ?? throw new NullReferenceException();
      lastModifiedBy.SetColumnName("last_modified_by");
    }
  }

  private static bool IsInherited(IConventionEntityType entityType)
    => entityType.ClrType is { IsAbstract: false, IsInterface: false } &&
      entityType.ClrType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILastModifiedEntityFields<>));
}
