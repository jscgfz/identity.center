using Identity.Center.Domain.Primitives.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Identity.Center.Persistence.Conventions;

internal sealed class MasterFieldsConventions : IModelFinalizingConvention
{
  public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
  {
    foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes().Where(IsInherited))
    {
      IConventionProperty name = entityType.FindProperty(nameof(IMasterFields.Name)) ?? throw new NullReferenceException();
      name.SetColumnName(nameof(name));
      name.Builder.HasColumnOrder(1);
      IConventionProperty description = entityType.FindProperty(nameof(IMasterFields.Description)) ?? throw new NullReferenceException();
      description.SetColumnName(nameof(description));
      description.Builder.HasColumnOrder(2);
      IConventionIndex index = entityType.AddIndex(name) ?? throw new NullReferenceException();
      index.SetIsUnique(true);
    }
  }

  public bool IsInherited(IConventionEntityType entityType)
    => entityType.ClrType is { IsAbstract: false, IsInterface: false } &&
      entityType.ClrType.GetInterfaces().Any(i => i == typeof(IMasterFields));
}
