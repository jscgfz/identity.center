using System.Reflection;
using FluentValidation;
using Identity.Center.Application.Result.Behaviors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Extensions;

public static class DependencyInjection
{
  public static WebApplicationBuilder WithResultExtensions(this WebApplicationBuilder builder)
  {
    Assembly assembly = Assembly.GetExecutingAssembly();
    builder
      .Services
      .AddMediatR(options =>
      {
        options.RegisterServicesFromAssembly(assembly);
        options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ResultBreakerBehavior<,>));
      });
    builder
      .Services
      .AddValidatorsFromAssembly(assembly);
    return builder;
  }
}
