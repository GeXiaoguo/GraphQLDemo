using GraphQL;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace GraphQLDemo.Controllers
{
    public class Droid
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class Query
    {
        private List<Droid> _droids = new List<Droid>
                                          {
                                            new Droid { Id = "123", Name = "R2-D2" },
                                            new Droid { Id = "456", Name = "R2-D1" },
                                            new Droid { Id = "789", Name = "R2-D3" }
                                          };

        [GraphQLMetadata("droid")]
        public Droid GetDroid(string id)
        {
            return _droids.FirstOrDefault(x => x.Id == id);
        }
        [GraphQLMetadata("droids")]
        public IEnumerable<Droid> GetDroids()
        {
            return _droids;
        }
    }
    public class GraphQLController : ApiController
    {
        // GET: api/GraphQL
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GraphQL/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/GraphQL
        public string Post([FromBody]string query)
        {
            var schema = Schema.For(@"
  type Droid {
    id: ID
    name: String
  }

  type Query {
    droid(id: ID): Droid
    droids: [Droid]
  }
", _ =>
            {
                _.Types.Include<Query>();
            });

            var json = schema.Execute(_ =>
            {
                _.Query = query;
            });

            return json;

        }

        // PUT: api/GraphQL/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GraphQL/5
        public void Delete(int id)
        {
        }
    }
}
