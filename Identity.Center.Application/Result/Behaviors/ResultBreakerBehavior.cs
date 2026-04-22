using System.Net;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Identity.Center.Application.Abstractions.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Result.Behaviors;

internal sealed class ResultBreakerBehavior<TRequest, TResponse>(IServiceProvider provider) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : notnull, IRequest<TResponse>
{
  private readonly IServiceProvider _provider = provider;
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    Type responseType = typeof(TResponse);
    Type requestType = typeof(TRequest);
    if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(Result<>))
      return await next(cancellationToken);

    if (
      requestType.GetInterfaces().FirstOrDefault(
        type => type.IsGenericType && new[] { typeof(ICommand<>), typeof(IQuery<>) }.Any(t => t == type.GetGenericTypeDefinition())
      ) is not Type resultRequestType
    )
      return await next(cancellationToken);

    IEnumerable<DbContext> contexts = _provider.GetServices<DbContext>();
    IEnumerable<IDbContextTransaction> transactions = resultRequestType.GetGenericTypeDefinition() == typeof(ICommand<>)
      ? await Task.WhenAll(contexts.Select(async c => c.Database.CurrentTransaction ?? await c.Database.BeginTransactionAsync(cancellationToken)))
      : [];
    IEnumerable<IValidator<TRequest>> validators = _provider.GetServices<IValidator<TRequest>>();

    try
    {
      if (validators.Any())
      {
        IEnumerable<ValidationResult> validations = await Task.WhenAll(
          validators.Select(v => v.ValidateAsync(request, cancellationToken))
        );

        IEnumerable<BaseError> errors = validations
          .SelectMany(v => v.Errors)
          .Select(e => new BaseError($"{e.PropertyName}.{e.ErrorCode}", e.ErrorMessage));

        if (errors.Any())
          return (TResponse)Activator.CreateInstance(responseType, null, HttpStatusCode.BadRequest, errors)!;
      }

      if (resultRequestType.GetGenericTypeDefinition() == typeof(IQuery<>))
        foreach (DbContext context in contexts)
          context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

      TResponse response = await next(cancellationToken);

      await Task.WhenAll(
        transactions.Select(t =>
          responseType.GetProperty(nameof(Result<Unit>.Success)) is PropertyInfo success && ((bool?)success.GetValue(response) ?? false)
          ? t.CommitAsync(cancellationToken)
          : t.RollbackAsync(cancellationToken)
        )
      );

      return response;
    }
    catch
    {
      await Task.WhenAll(
        transactions.Select(t => t.RollbackAsync(cancellationToken))
      );

      throw;
    }
  }
}
