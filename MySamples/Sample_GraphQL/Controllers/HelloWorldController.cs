using GraphQL;
using GraphQL.Types;
using GraphQL.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Sample_GraphQL.Queries;
using Sample_GraphQL.GraphTypes;

namespace Sample_GraphQL.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloWorldController : ControllerBase
{
  private readonly ILogger<HelloWorldController> _logger;

  public HelloWorldController(ILogger<HelloWorldController> logger)
  {
    _logger = logger;
  }

  [HttpGet]
  public async Task<ActionResult<object>> GetHelloWorld()
  {
    Schema schema = Schema.For(@"
      type Query {
        hello: String
      }
    ");

    return await schema.ExecuteAsync(_ => {
      _.Query = "{ hello }";
      _.Root = new { Hello = "Hello World!" };
    });
  }

  [HttpGet("Hero")]
  public async Task<ActionResult<object>> GetHero()
  {
    Schema schema = Schema.For(@"
      type Droid {
        id: ID!
        name: String!
      }

      type Query {
        hero: Droid
      }
    ", _ => {
      _.Types.Include<Query>();
    });

    return await schema.ExecuteAsync(_ => {
      _.Query = "{ hero { id name } }";
    });
  }

  [HttpGet("StarWars")]
  public async Task<ActionResult<object>> GetStarWars()
  {
    var schema = new Schema { Query = new StarWarsQuery() };
    
    return await schema.ExecuteAsync(_ => {
      _.Query = "{ hero { id name } }";
    });
  }

  [HttpGet("Friend")]
  public async Task<ActionResult<object>> GetFriend()
  {
    Schema schema = Schema.For(@"
      type Droid {
        id: Int!
        name: String!
        friend: Character
      }
      
      type Character {
        name: String!
      }
      
      type Query {
        hero: Droid
      }
    ", _ => {
      _.Types.Include<DroidType>();
      _.Types.Include<Query>();
    });

    return await schema.ExecuteAsync(_ => {
      _.Query = "{ hero { id name friend { name } } }";
    });
  }
}
