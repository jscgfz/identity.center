using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal sealed class ActionConfiguration : IEntityTypeConfiguration<Action>
{
  public void Configure(EntityTypeBuilder<Action> builder)
  {
    builder
      .ToTable("actions", IdentitySchemas.Security);
  }
}
