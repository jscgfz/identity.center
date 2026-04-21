using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Application.Abstractions.Result;

public interface IQuery<T> : IRequest<Result<T>>
{ }
