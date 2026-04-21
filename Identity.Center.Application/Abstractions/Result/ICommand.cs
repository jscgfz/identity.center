using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Application.Abstractions.Result;
public interface ICommand<T> : IRequest<Result<T>>
{ }
