using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Identity;

public class User : Entity<Guid>
{
  public required string DocumentType { get; set; }
  public required string DocumentNumber { get; set; }
  public required string FirstName { get; set; }
  public string? SecondName { get; set; }
  public required string FirstLastName { get; set; }
  public string? SecondLastName { get; set; }

  public virtual ICollection<ContactInfo> ContactInfo { get; set; } = [];
  public virtual ICollection<DomainCredential> DomainCredentials { get; set; } = [];
  public virtual ICollection<SingleCredential> SingleCredentials { get; set; } = [];
  public virtual ICollection<UserRole> Roles { get; set; } = [];
}
