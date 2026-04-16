using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Enums;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Security;

public class ContactType : MasterEntity<Guid>
{
  public required ContactTypes ContactTypeKey { get; set; }
  public virtual ICollection<ContactInfo> ContactInfo { get; set; } = [];
}
