using Identity.Center.Domain.Primitives.Abstractions;
using Identity.Center.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Identity.Center.Persistence.Conventions;

internal sealed class KeyedEntityConvention : IModelFinalizingConvention
{
  public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
  {
    foreach (IConventionProperty conventionProperty in modelBuilder.Metadata.GetEntityTypes().Where(IsInherited).Select(GetId))
    {
      conventionProperty.SetColumnName("id");
      conventionProperty.Builder.HasColumnOrder(0);
      _ = conventionProperty.ClrType switch
      {
        Type t when t == typeof(Guid) => SetGuid(conventionProperty),
        Type t when new[] { typeof(int), typeof(string) }.Any(type => type == t) => string.Empty,
        _ => throw new NotImplementedException(conventionProperty.ClrType.Name)
      };
    }
  }

  private static bool IsInherited(IConventionEntityType entityType)
    => entityType.ClrType is { IsAbstract: false, IsInterface: false } &&
      entityType.ClrType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IKeyedEntity<>));

  private static IConventionProperty GetId(IConventionEntityType entityType)
    => entityType.FindProperty(nameof(IKeyedEntity<Guid>.Id)) ?? throw new NullReferenceException();

  private static string? SetGuid(IConventionProperty conventionProperty)
  {
    conventionProperty.SetValueGenerated(ValueGenerated.OnAddOrUpdate);
    return conventionProperty.SetDefaultValueSql(IdentityDefaultValues.Guid);
  }
}
