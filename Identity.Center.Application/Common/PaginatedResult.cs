using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Result;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Common;

public sealed record PaginatedResult<TData>(
  int Count,
  int PageCount,
  int PageIndex,
  int PageSize,
  IEnumerable<TData> Data
) : IPaginatedResult<TData> where TData : class;

public static class PaginatedResult
{
  public static async Task<IPaginatedResult<TValue>> ComputeAsync<TValue, TParams>(
    IQueryable<TValue> set,
    TParams @params,
    CancellationToken cancellationToken = default
  )
      where TValue : class
      where TParams : IPaginationParams
  {
    int pageIndex = @params.PageIndex ?? 1;
    int pageSize = @params.PageSize ?? 10;
    bool fullset = @params.FullSet ?? false;

    int total = await set.CountAsync(cancellationToken);
    int pagesCount = (int)Math.Ceiling((double)total / pageSize);

    IEnumerable<TValue> data = await (fullset ? set : set.Skip((pageIndex - 1) * pageSize).Take(pageSize)).ToListAsync(cancellationToken);
    return new PaginatedResult<TValue>(
      total,
      fullset ? 1 : pagesCount,
      fullset ? 1 : pageIndex,
      fullset ? total : pageSize,
      data
    );
  }

  public static Result<IPaginatedResult<TData>> AsResult<TData>(this IPaginatedResult<TData> set)
    where TData : class
    => set.Data.Any()
    ? new Result<IPaginatedResult<TData>>(set, System.Net.HttpStatusCode.OK, [])
    : Result.Result.Failure<IPaginatedResult<TData>>(System.Net.HttpStatusCode.NotFound, new BaseError("Data.NotFound", "No se encontro información"));
}
