using System.Net;
using Identity.Center.Application.Abstractions.Reponses;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Identity.Center.Application.Result;

public readonly struct Result<T>
{
  private readonly T? _value;
  private readonly int _statusCode;
  private readonly IEnumerable<IError> _errors;
  private readonly bool _success;

  public readonly bool Success => _success;
  public readonly bool Failed => !_success;
  public readonly int Code => _statusCode;
  public readonly IEnumerable<IError> Errors => _errors;
  public readonly T Value => Success && _value != null ? _value : throw new InvalidOperationException(nameof(Value));

  public Result(T? value, int statusCode, params IEnumerable<IError> errors)
  {
    ArgumentNullException.ThrowIfNull(statusCode == default ? null : statusCode, nameof(statusCode));
    if (!Enum.GetValues<HttpStatusCode>().Any(code => ((int)code) == statusCode)) throw new InvalidOperationException(nameof(statusCode));
    _statusCode = statusCode;
    _success = _statusCode >= 100 && _statusCode <= 399;
    if (_success) ArgumentNullException.ThrowIfNull(value, nameof(value));
    else if (!errors.Any()) throw new ArgumentException($"${nameof(errors)} required", nameof(errors));
    _value = value;
    _errors = errors;
  }

  public Result(T? value, ResultTypes statusCode, IEnumerable<IError> errors)
    : this(value, ((int)statusCode), errors) { }

  public Result(T? value, HttpStatusCode statusCode, IEnumerable<IError> errors)
    : this(value, ((int)statusCode), errors) { }

  public static implicit operator Result<T>(T value)
    => value is not null
    ? new(value, HttpStatusCode.OK, [])
    : new(value, HttpStatusCode.NotFound, [IdentityErrors.NotFound]);
}

public static class Result
{
  public static Result<Unit> Unit => new(Unit.Value, HttpStatusCode.OK, []);
  public static Result<T> Success<T>(this T value) => new(value, HttpStatusCode.OK, []);
  public static Result<T> Failure<T>(int statusCode, params IEnumerable<IError> errors)
    => new(default, statusCode, errors);
  public static Result<T> Failure<T>(ResultTypes statusCode, params IEnumerable<IError> errors)
    => new(default, statusCode, errors);
  public static Result<T> Failure<T>(HttpStatusCode statusCode, params IEnumerable<IError> errors)
    => new(default, statusCode, errors);
  public static Result<T> Failure<T>(params IEnumerable<IError> errors)
    => new(default, HttpStatusCode.Conflict, errors);

  public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> factory)
    => result.Failed ? Failure<TOut>(result.Code, result.Errors) : factory.Invoke(result.Value);
  public static async Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Result<TOut>> factory)
    => Bind(await resultTask, factory);
  public static async Task<Result<TOut>> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> asyncFactory)
    => result.Failed ? Failure<TOut>(result.Code, result.Errors) : await asyncFactory.Invoke(result.Value);
  public static async Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> asyncFactory)
    => await Bind(await resultTask, asyncFactory);

  public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> factory)
    => result.Failed ? Failure<TOut>(result.Code, result.Errors) : new(factory.Invoke(result.Value), result.Code, []);
  public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> factory)
    => Map(await resultTask, factory);

  public static async Task<Result<TOut>> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<TOut>> asyncFactory)
    => result.Failed ? Failure<TOut>(result.Code, result.Errors) : new(await asyncFactory.Invoke(result.Value), result.Code, []);
  public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<TOut>> asyncFactory)
    => await Map(await resultTask, asyncFactory);
  public static Result<TOut> Merge<TIn1, TIn2, TOut>(Result<TIn1> result1, Result<TIn2> result2, Func<TIn1, TIn2, TOut> factory)
  {
    if (result1.Failed || result2.Failed)
      return result1.Failed
        ? Failure<TOut>(result1.Code, result1.Errors)
        : Failure<TOut>(result2.Code, result2.Errors);

    return new(factory.Invoke(result1.Value, result2.Value), result1.Code, []);
  }
  public static async Task<Result<TOut>> Merge<TIn1, TIn2, TOut>(Task<Result<TIn1>> resultTask1, Task<Result<TIn2>> resultTask2, Func<TIn1, TIn2, Task<TOut>> asyncFactory)
  {
    Result<TIn1> result1 = await resultTask1;
    Result<TIn2> result2 = await resultTask2;

    if (result1.Failed || result2.Failed)
      return result1.Failed
        ? Failure<TOut>(result1.Code, result1.Errors)
        : Failure<TOut>(result2.Code, result2.Errors);

    return new(await asyncFactory.Invoke(result1.Value, result2.Value), result1.Code, []);
  }

  public static IResult AsHttpResult<TIn>(this Result<TIn> result)
  {
    if (result.Success)
      return result.Value switch
      {
        ICreatedResponse created => Results.Created(default(string), created),
        IFileResponse file => RenderFile(file),
        Unit unit => Results.NoContent(),
        _ => Results.Ok(result.Value)
      };

    return Results.Problem(
      $"StatusCode - {result.Code} - " + Enum.GetName((HttpStatusCode)result.Code) ?? "Unrecognized",
      null,
      result.Code,
      Enum.GetName((HttpStatusCode)result.Code) ?? "Unrecognized",
      null,
      new Dictionary<string, object?>()
      {
        ["errors"] = result.Errors
          .Select(e => e.Seralize())
          .GroupBy(e => e.Key)
          .ToDictionary(e => e.Key, e => e.Select(i => i.Value))
      }
    );
  }

  private static IResult RenderFile<TFile>(TFile file)
    where TFile : IFileResponse
  {
    FileContentResponse content = file.Render();
    return Results.File(
      content.Content,
      content.MimeType,
      content.Name
    );
  }

  public static async Task<IResult> AsHttpResult<TIn>(this Task<Result<TIn>> result)
    => AsHttpResult(await result);
}