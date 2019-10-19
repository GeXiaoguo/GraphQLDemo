using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Character
    {
        // no explicit resolvers defined for this field. Default to the property of the poco class itself.
        public string Name { get; set; }
    }
    public class Droid
    {
        public string Id { get; set; }
        public string Name { get; set; }

        // not defined in the schema. Not queryable by the client.
        public DateTime Birthday { get; set; }
    }

    [GraphQLMetadata("Droid", IsTypeOf = typeof(DroidFieldResolvers))]
    public class DroidFieldResolvers
    {
        // this resolver returns the poco class property. It is the default behavior already. No need to define this resolver. 
        public string Id(Droid droid) => droid.Id;

        // this resolver returns a computer value different than the poco property.
        public string Name(Droid droid) => droid.Name + "fake";

        // these two parameters are optional
        // ResolveFieldContext provides contextual information about the field
        public Character Friend(ResolveFieldContext context, Droid source)
        {
            return new Character { Name = $"C3-PO. {this.Name(source)}  is my friend" };
        }
    }

    [GraphQLMetadata("Query", IsTypeOf = typeof(QueryResolvers))]

    public class QueryResolvers
    {
        [GraphQLMetadata("getDroid")]
        public Droid GetDroid(string id)
        {
            return new[]
            {
                new Droid { Id = "123", Name = "R2-D2" },
                new Droid { Id = "456", Name = "R3-D3" }
            }
            .FirstOrDefault(x => x.Id == id);
        }

        [GraphQLMetadata("getDroids")]
        public IEnumerable<Droid> GetDroids()
        {
            return new[]
            {
                new Droid { Id = "123", Name = "R2-D2" },
                new Droid { Id = "456", Name = "R3-D3" }
            };
        }
    }
    class Program
    {
        const string _schema = @"type Droid {
                                      id: ID!
                                      name: String!
                                      friend: Character
                                    }

                                    type Character {
                                      name: String!
                                    }

                                    type Query {
                                      getDroid(id: ID): Droid
                                      getDroids: [Droid]
                                    }";
        static void Main(string[] args)
        {
            var schema = Schema.For(_schema, schemaBuilder =>
            {
                schemaBuilder.Types.Include<Droid>();
                schemaBuilder.Types.Include<Character>();

                schemaBuilder.Types.Include<QueryResolvers>();
                schemaBuilder.Types.Include<DroidFieldResolvers>();
            });

            var json = schema.Execute(executionOptions =>
            {
                executionOptions.Query = $"{{ getDroid(id: \"123\") {{ id name }} }}";
            });
            Console.WriteLine(json);

            json = schema.Execute(executionOptions =>
            {
                executionOptions.Query = $"{{ getDroids {{ id name friend {{name}} }} }}";
            });
            Console.WriteLine(json);
        }
    }
}
