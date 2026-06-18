using System.Reflection;
using FluentValidation;
using Identity.Center.Application.Result.Behaviors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Identity.Center.Application.Extensions;

public static class DependencyInjection
{
  public static WebApplicationBuilder WithResultExtensions(this WebApplicationBuilder builder)
  {
    Assembly assembly = Assembly.GetExecutingAssembly();
    foreach (Type type in assembly.GetTypes().Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))))
      builder.Services.AddTransient(
        type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)),
        type
      );
    builder
      .Services
      .AddMediatR(options =>
      {
        options.RegisterServicesFromAssembly(assembly);
        options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ResultBreakerBehavior<,>));
      });
    return builder;
  }

  public static StreamPart AsStreamPart(this IFormFile file)
    => new(file.OpenReadStream(), file.FileName, file.ContentType);
}
