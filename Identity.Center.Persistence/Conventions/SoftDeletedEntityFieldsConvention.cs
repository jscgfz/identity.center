using System.Linq.Expressions;
using Identity.Center.Domain.Primitives.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Identity.Center.Persistence.Conventions;

internal sealed class SoftDeletedEntityFieldsConvention : IModelFinalizingConvention
{
  public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
  {
    foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes().Where(IsInherited))
    {
      IConventionProperty isDeleted = entityType.FindProperty(nameof(ISoftDeletedEntityFields<Guid>.IsDeleted)) ?? throw new NullReferenceException();
      isDeleted.SetColumnName("deleted");
      isDeleted.SetDefaultValue(false);
      IConventionProperty deletedAt = entityType.FindProperty(nameof(ISoftDeletedEntityFields<Guid>.DeletedAtUtc)) ?? throw new NullReferenceException();
      deletedAt.SetColumnName("deleted_at_utc");
      IConventionProperty deletedBy = entityType.FindProperty(nameof(ISoftDeletedEntityFields<Guid>.DeletedBy)) ?? throw new NullReferenceException();
      deletedBy.SetColumnName("deleted_by");

      ParameterExpression param = Expression.Parameter(entityType.ClrType, "row");
      MemberExpression prop = Expression.PropertyOrField(param, nameof(ISoftDeletedEntityFields<Guid>.IsDeleted));
      LambdaExpression exp = Expression.Lambda(Expression.Equal(prop, Expression.Constant(false)), param);
      entityType.SetQueryFilter(exp);
    }
  }

  private static bool IsInherited(IConventionEntityType entityType)
    => entityType.ClrType is { IsAbstract: false, IsInterface: false } &&
      entityType.ClrType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISoftDeletedEntityFields<>));
}
