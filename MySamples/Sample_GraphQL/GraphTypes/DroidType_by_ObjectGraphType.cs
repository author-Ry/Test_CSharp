using GraphQL.Types;
using Sample_GraphQL.Models;

namespace Sample_GraphQL.GraphTypes;

public class DroidType_by_ObjectGraphType : ObjectGraphType<Droid>
{
  public DroidType_by_ObjectGraphType()
  {
    Field(f => f.Id).Description("The Id of the Droid.");
    Field(f => f.Name).Description("The name of the Droid.");
  }
}