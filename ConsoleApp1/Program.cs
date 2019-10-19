using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Character
    {
        public string Name { get; set; }
    }
    public class Droid
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [GraphQLMetadata("Droid", IsTypeOf = typeof(Droid))]
    public class DroidType
    {
        public string Id(Droid droid) => droid.Id;
        public string Name(Droid droid) => droid.Name + "fake";

        // these two parameters are optional
        // ResolveFieldContext provides contextual information about the field
        public Character Friend(ResolveFieldContext context, Droid source)
        {
            return new Character { Name = "C3-PO" };
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
                                    }

                                    type Query {
                                      getDroid(id: ID): Droid
                                      getDroids: [Droid]
                                    }";
        static void Main(string[] args)
        {
            var schema = Schema.For(_schema, schemaBuilder =>
            {
                schemaBuilder.Types.Include<QueryResolvers>();
                schemaBuilder.Types.Include<Droid>();
                schemaBuilder.Types.Include<DroidType>();
            });

            var json = schema.Execute(executionOptions =>
            {
                executionOptions.Query = $"{{ getDroid(id: \"123\") {{ id name }} }}";
            });

            json = schema.Execute(executionOptions =>
            {
                executionOptions.Query = $"{{ getDroids {{ id name }} }}";
            });
            Console.WriteLine(json);
        }
    }
}
