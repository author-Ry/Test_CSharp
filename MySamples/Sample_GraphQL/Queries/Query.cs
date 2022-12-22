using GraphQL;
using Sample_GraphQL.Models;

namespace Sample_GraphQL.Queries;

public class Query
{
  [GraphQLMetadata("hero")]
  public Droid GetHero()
    => new Droid { Id = 1, Name = "R2-D2" };
}