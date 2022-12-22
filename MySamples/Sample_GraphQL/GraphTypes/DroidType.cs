using GraphQL;
using Sample_GraphQL.Models;

namespace Sample_GraphQL.GraphTypes;

[GraphQLMetadata("Droid", IsTypeOf = typeof(Droid))]
public class DroidType
{
  public int? Id([FromSource] Droid droid) => droid.Id;
  public string? Name([FromSource] Droid droid) => droid.Name;

  public Character Friend(IResolveFieldContext context, [FromSource] Droid source)
  {
    return new Character { Name = "C3-PO" };
  }
}