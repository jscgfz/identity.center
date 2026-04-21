using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Application.Abstractions.Result;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
  where TQuery : IQuery<TResponse>
{ }