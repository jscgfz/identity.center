using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Center.Application.Abstractions.Managers;

public interface IBaseUnitOfWork
{
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  IDbContextTransaction? CurrentTransaction { get; }
}
