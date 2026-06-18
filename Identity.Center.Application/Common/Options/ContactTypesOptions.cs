namespace Identity.Center.Application.Common.Options;

public sealed class ContactTypesOptions
{
  public required IEnumerable<string> CellPhoneExpressions { get; set; }
  public required IEnumerable<string> EmailExpressions { get; set; }
}
