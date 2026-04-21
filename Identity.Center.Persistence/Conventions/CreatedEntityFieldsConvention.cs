using Identity.Center.Domain.Primitives.Abstractions;
using Identity.Center.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Identity.Center.Persistence.Conventions;

internal sealed class CreatedEntityFieldsConvention : IModelFinalizingConvention
{
  public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
  {
    foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes().Where(IsInherited))
    {
      IConventionProperty createdAt = entityType.FindProperty(nameof(ICreatedEntityFields<Guid>.CreatedAtUtc)) ?? throw new NullReferenceException();
      createdAt.SetColumnName("created_at_utc");
      IConventionProperty createdBy = entityType.FindProperty(nameof(ICreatedEntityFields<Guid>.CreatedBy)) ?? throw new NullReferenceException();
      createdBy.SetColumnName("created_by");
      createdAt.SetDefaultValueSql(IdentityDefaultValues.UtcNow);
      _ = createdBy.ClrType is { IsGenericType: true } && createdBy.ClrType.GetGenericTypeDefinition() == typeof(Nullable<>) ? createdBy.ClrType.GetGenericArguments().First() : createdBy.ClrType switch
      {
        Type t when t == typeof(Guid) => createdBy.SetDefaultValue(Guid.Empty),
        Type t when t == typeof(int) => createdBy.SetDefaultValue(0),
        Type t when t == typeof(string) => createdBy.SetDefaultValue(string.Empty),
        _ => throw new NotImplementedException(createdBy.ClrType.Name)
      };
    }
  }

  private static bool IsInherited(IConventionEntityType entityType)
    => entityType.ClrType is { IsAbstract: false, IsInterface: false } &&
      entityType.ClrType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICreatedEntityFields<>));
}
