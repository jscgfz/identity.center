using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common.Response;
using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.Types.Queries.GetContactTypes;

public sealed record GetContactTypesQuery() : IQuery<IEnumerable<MasterOption<ContactTypes>>>;
