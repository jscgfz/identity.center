using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common.Response;

namespace Identity.Center.Application.Features.Types.Queries.GetCredentialTypes;

public sealed record GetCredentialTypesQuery() : IQuery<IEnumerable<MasterOption<int>>>;
