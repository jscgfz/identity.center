using Identity.Center.Application.Result;

namespace Identity.Center.Application.Abstractions.Common;

public interface IPaginatedResult<TData>
  where TData : class
{
  int Count { get; }
  int PageCount { get; }
  int PageIndex { get; }
  int PageSize { get; }
  IEnumerable<TData> Data { get; }
}
