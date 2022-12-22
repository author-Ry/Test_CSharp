using GraphQL.Types;
using Sample_GraphQL.GraphTypes;
using Sample_GraphQL.Models;

namespace Sample_GraphQL.Queries;

public class StarWarsQuery : ObjectGraphType
{
  public StarWarsQuery()
  {
    Field<DroidType_by_ObjectGraphType>("hero").Resolve(context => new Droid { Id = 1, Name = "R2-D2" });
  }
}