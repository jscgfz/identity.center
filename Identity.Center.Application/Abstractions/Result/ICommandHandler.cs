using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Application.Abstractions.Result;

public interface ICommandHandler<in TCommant, TResponse> : IRequestHandler<TCommant, Result<TResponse>>
  where TCommant : ICommand<TResponse>
{ }
