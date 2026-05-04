namespace Identity.Center.Application.Common.Response;

public sealed record MasterOption<T>(
  T Value,
  string Label
);
