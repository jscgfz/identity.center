using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
  public void Configure(EntityTypeBuilder<Group> builder)
  {
    builder
      .ToTable("groups", IdentitySchemas.Security);
  }
}
