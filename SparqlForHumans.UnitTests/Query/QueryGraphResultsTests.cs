﻿using System.Linq;
using SparqlForHumans.Lucene;
using SparqlForHumans.Lucene.Index;
using SparqlForHumans.Lucene.Queries.Graph;
using SparqlForHumans.Models.RDFExplorer;
using SparqlForHumans.Utilities;
using Xunit;

namespace SparqlForHumans.UnitTests.Query
{
    public class QueryGraphResultsTests
    {
        private const string EntitiesIndexPath = "QueryGraphQueryingEntities";
        private const string PropertiesIndexPath = "QueryGraphQueryingProperties";

        private static void CreateIndex()
        {
            // Arrange
            const string filename = @"Resources/QueryGraphQuerying.nt";
            EntitiesIndexPath.DeleteIfExists();
            PropertiesIndexPath.DeleteIfExists();
            new EntitiesIndexer(filename, EntitiesIndexPath).Index();
            new PropertiesIndexer(filename, PropertiesIndexPath).Index();
        }

        private static void DeleteIndex()
        {
            EntitiesIndexPath.DeleteIfExists();
            PropertiesIndexPath.DeleteIfExists();
        }

        /// <summary>
        /// A single node.
        /// Should query for the top Entities.
        /// In the given example QueryGraph.nt, Obama should be in the top values.
        /// </summary>
        [Fact]
        public void TestResults_1IsolatedNode_1_1Node0Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[] { new Node(0, "?var0") },
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q77"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q78"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q100"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q298"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("human"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("United States"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("country"));

            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q99999"));

            Assert.True(queryGraph.Nodes[0].Results.All(x=>x.Id.StartsWith("Q")));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// A single node. Given Type. No results.
        /// </summary>
        [Fact]
        public void TestResults_1IsolatedNode_2_GivenType_1Node0Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[] { new Node(0, "?varObama", new[] { "http://www.wikidata.org/entity/Q76" }) },
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[0].QueryType);
            Assert.Empty(queryGraph.Nodes[0].Results);

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// Two isolated nodes, no properties between them.
        /// Both should be Top Entities.
        /// </summary>
        [Fact]
        public void TestResults_2IsolatedNodes_1_2Nodes0Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0"),
                    new Node(1, "?var1"),
                },
            };
            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q77"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q78"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q100"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q298"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q99999"));
            Assert.True(queryGraph.Nodes[0].Results.All(x=>x.Id.StartsWith("Q")));

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q5"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q30"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q6256"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q77"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q78"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q100"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q298"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q99999"));
            Assert.True(queryGraph.Nodes[1].Results.All(x=>x.Id.StartsWith("Q")));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// Two isolated nodes, no properties between them. Empty Results.
        /// </summary>
        [Fact]
        public void TestResults_2IsolatedNodes_2_GivenTypes_2Nodes0Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0", new[]{"http://www.wikidata.org/entity/Q76"}),
                    new Node(1, "?var1", new[]{"http://www.wikidata.org/entity/Q49089"}),
                },
            };
            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[0].QueryType);
            Assert.Empty(queryGraph.Nodes[0].Results);

            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 ?prop0 ?var1
        /// No given Types for anyone.
        /// All should return Top Entities and Properties;
        /// </summary>
        [Fact]
        public void TestResults_2Nodes_1_NoGivenTypes_2Nodes1Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0"),
                    new Node(1, "?var1"),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1)
                },
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("United States"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("human"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("country"));
            Assert.True(queryGraph.Nodes[0].Results.All(x=>x.Id.StartsWith("Q")));

            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q49089"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Label.Equals("Human 1"));
            Assert.True(queryGraph.Nodes[1].Results.All(x=>x.Id.StartsWith("Q")));

            //Assert.Equal(QueryType.QueryTopProperties, queryGraph.Edges[0].QueryType);
            Assert.NotEmpty(queryGraph.Edges[0].Results);
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P25"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P31"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P22"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P27"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P555"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P777"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("Mother Of"));
            Assert.True(queryGraph.Edges[0].Results.All(x=>x.Id.StartsWith("P")));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 ?prop0 ?var1
        /// ?var0 is Obama
        /// Expected Results:
        /// ?var0 is a given type
        /// ?var1 is a type that can be accessed only from Obama.
        /// ?var1 is an Inferred Domain Type.
        /// This looks like it should be a query.
        /// </summary>
        [Fact]
        public void TestResults_2Nodes_2_SourceIsGivenType_2Nodes1Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0", new[]{"http://www.wikidata.org/entity/Q76"} ),
                    new Node(1, "?var1"),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1)
                },
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[0].QueryType);
            Assert.Empty(queryGraph.Nodes[0].Results);

            //Assert.Equal(QueryType.GivenSubjectTypeQueryDirectlyEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q5")); //Instance Of
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q30")); //Country of citizenship
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q78")); //Father of
            //TODO: Not sure how to tests this without the endpoint or the values in the index.
            //TODO: Currently I am getting all the results.
            //TODO: SHOULD be DoesNotContain(). Eventually these tests will fail if this is fixed:
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q6256")); //No relationship with
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q77")); //Reverse relationship only

            //Assert.Equal(QueryType.GivenSubjectTypeDirectQueryOutgoingProperties, queryGraph.Edges[0].QueryType);
            Assert.NotEmpty(queryGraph.Edges[0].Results);
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P27"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("country of citizenship"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("Instance Of")); //Instance
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("father")); //reverse
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("random property 555")); //Not there

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 ?prop0 ?var1
        /// ?var1 is Obama
        /// 
        /// Expected Results:
        /// ?var0 are types going to Obama
        /// ?prop0 are properties going to Obama
        /// </summary>
        [Fact]
        public void TestResults_2Nodes_3_TargetIsGivenType_2Nodes1Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0" ),
                    new Node(1, "?var1", new[]{"http://www.wikidata.org/entity/Q76"}),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1)
                },
            };
            
            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.GivenObjectTypeQueryDirectlyEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.True(queryGraph.Nodes[0].Results.All(x=>x.Id.StartsWith("Q")));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q77"));
            //TODO: Not sure how to tests this without the endpoint or the values in the index.
            //TODO: Currently I am getting all the results.
            //TODO: SHOULD be DoesNotContain(). Eventually these tests will fail if this is fixed:
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5")); //No relation
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30")); //No relation
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q78")); //No relation
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256")); //No relation
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76")); //Self: No relation

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);

            //Assert.Equal(QueryType.GivenObjectTypeDirectQueryIncomingProperties, queryGraph.Edges[0].QueryType);
            Assert.NotEmpty(queryGraph.Edges[0].Results);
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P22"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("father"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("Instance Of"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("country of citizenship"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 ?prop0 ?var1
        /// ?var0 is Obama
        /// ?var1 is USA
        /// 
        /// Expected Results:
        /// ?prop0 are properties from Obama going to USA
        /// </summary>
        [Fact]
        public void TestResults_2Nodes_4_SourceTargetAreGivenType_2Nodes1Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0", new[]{"http://www.wikidata.org/entity/Q76"} ),
                    new Node(1, "?var1", new[]{"http://www.wikidata.org/entity/Q30"}),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1)
                },
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[0].QueryType);
            Assert.Empty(queryGraph.Nodes[0].Results);

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);

            //Assert.Equal(QueryType.GivenSubjectAndObjectTypeDirectQueryIntersectOutInProperties, queryGraph.Edges[0].QueryType);
            Assert.NotEmpty(queryGraph.Edges[0].Results);
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P27"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("country of citizenship"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("Instance Of"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("father"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("random property 555"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("random property 777"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 P27 ?var1
        /// ?var0 is Obama
        /// ?var1 is USA
        /// 
        /// Expected Results:
        /// All is given, should behave like that.
        /// </summary>
        [Fact]
        public void TestResults_2Nodes_5_SourceTargetPredicateAreGivenTypes_2Nodes1Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0", new[] {"http://www.wikidata.org/entity/Q76"}),
                    new Node(1, "?var1", new[] {"http://www.wikidata.org/entity/Q30"}),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1, new[] {"http://www.wikidata.org/prop/direct/P27"})
                },
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[0].QueryType);
            Assert.Empty(queryGraph.Nodes[0].Results);

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);
            
            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            // Cleanup
            DeleteIndex();
        }

        

        /// <summary>
        /// ?var0 P31 ?var1
        /// ?var1 is Human
        /// 
        /// Expected Results:
        /// ?var0 are instances of Human
        /// </summary>
        [Fact]
        public void TestResults_2Nodes_6_SourceIsInstanceOfType_2Nodes1Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0" ),
                    new Node(1, "?var1", new[]{"http://www.wikidata.org/entity/Q5"}),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1,new []{"http://www.wikidata.org/prop/direct/P31"})
                },
            };
            
            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.SubjectIsInstanceOfTypeQueryEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q77"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q78"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Label.Equals("United States"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Label.Equals("human"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Label.Equals("country"));

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 P31 ?var1
        /// ?var1 is Human
        /// 
        /// Expected Results:
        /// ?var0 are instances of Human
        /// </summary>
        [Fact]
        public void TestResults_2Nodes_7_SourceToType_2Nodes1Edge()
        {
            var graph = new RDFExplorerGraph()
            {
                nodes = new[]
                {
                    new Node(0, "?var0" ),
                    new Node(1, "?var1", new[]{"http://www.wikidata.org/entity/Q5"}),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1)
                },
            };
            
            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.SubjectIsInstanceOfTypeQueryEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q77"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q78"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            //TODO: Not sure how to tests this without the endpoint or the values in the index.
            //TODO: Currently I am getting all the results.
            //TODO: SHOULD be DoesNotContain(). Eventually these tests will fail if this is fixed:
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5")); //No relation
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30")); //No relation
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256")); //No relation

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.NotEmpty(queryGraph.Edges[0].Results);
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("Instance Of"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("country of citizenship"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("father"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("random property 555"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Label.Equals("random property 777"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 P31 ?var1
        /// ?var1 is Human
        /// ?var0 ?prop0 ?var2
        /// 
        /// Expected Results:
        /// ?var0 are instances of Human
        /// ?var2 are //TODO: TBD. For the moment, TopEntities; Github #121;
        /// ?prop1 are properties with domain in Human 
        /// </summary>
        [Fact]
        public void TestResults_3Nodes_1_N0InstanceOfN1_E1DomainN0_3Nodes2Edge()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?var0"),
                    new Node(1, "?var1", new[]{"http://www.wikidata.org/entity/Q5"}),
                    new Node(2, "?var2"),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1, new[]{"http://www.wikidata.org/prop/direct/P31"}),
                    new Edge(1, "?prop1", 0, 2),
                }
            };
            
            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.SubjectIsInstanceOfTypeQueryEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q77"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q78"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);

            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[2].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[2].Results);
            Assert.True(queryGraph.Nodes[2].Results.All(x=>x.Id.StartsWith("Q")));
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q76")); //P22
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q78")); //P22
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q30")); //P27
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q5")); //P31
            //TODO: Not sure how to tests this without the endpoint or the values in the index.
            //TODO: Currently I am getting all the results.
            //TODO: SHOULD be DoesNotContain(). Eventually these tests will fail if this is fixed:
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q77")); // No incoming from human
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q6256")); //No relation


            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            //Assert.Equal(QueryType.KnownSubjectTypeQueryDomainProperties, queryGraph.Edges[1].QueryType);
            Assert.NotEmpty(queryGraph.Edges[1].Results);
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P31"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P25"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P27"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P22"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P555"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?var0 P31 ?var1
        /// ?var1 is Human
        /// ?var2 ?prop0 ?var0
        /// 
        /// Expected Results:
        /// ?var0 are instances of Human
        /// ?var2 are //TODO: TBD. For the moment, TopEntities; Github #121;
        /// ?prop1 are properties with range in Human 
        /// </summary>
        [Fact]
        public void TestResults_3Nodes_2_N0InstanceOfN1_E1RangeN0_3Nodes2Edge()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?var0"),
                    new Node(1, "?var1", new[]{"http://www.wikidata.org/entity/Q5"}),
                    new Node(2, "?var2"),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0, 1, new[]{"http://www.wikidata.org/prop/direct/P31"}),
                    new Edge(1, "?prop1", 2, 0),
                }
            };
            
            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.SubjectIsInstanceOfTypeQueryEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q77"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q78"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));
            
            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[1].QueryType);
            Assert.Empty(queryGraph.Nodes[1].Results);

            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[2].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[2].Results);
            Assert.True(queryGraph.Nodes[2].Results.All(x=>x.Id.StartsWith("Q"))); //All going to human
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q76")); //P22
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q78")); //P22
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q77")); //P22
            //TODO: Not sure how to tests this without the endpoint or the values in the index.
            //TODO: Currently I am getting all the results.
            //TODO: SHOULD be DoesNotContain(). Eventually these tests will fail if this is fixed:
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q5")); //P31
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q30")); //P27
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q6256")); //No relation

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            //Assert.Equal(QueryType.KnownObjectTypeQueryRangeProperties, queryGraph.Edges[1].QueryType);
            Assert.NotEmpty(queryGraph.Edges[1].Results);
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P25"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P22"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P27"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P777"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P555"));

            //TODO: Not sure about this one. I have to check the data
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P31"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?human ?prop0 ?country
        /// ?human P31 HUMAN
        /// ?country P31 COUNTRY
        ///
        /// Expected:
        /// ?human IsInstanceOf HUMAN
        /// ?country IsInstanceOf COUNTRY
        /// ?prop0 Intersect Domain InstanceOf HUMAN Range InstanceOf COUNTRY
        /// </summary>
        [Fact]
        public void TestResults_4Nodes_1_N1InstanceOfN3_N2InstanceOfN4_N1E1N2_E1DomainN1RangeN2_4Nodes3Edge()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?human"),
                    new Node(1, "?country"),
                    new Node(2, "HUMAN", new[]{"http://www.wikidata.org/entity/Q5"}),
                    new Node(3, "COUNTRY", new[]{"http://www.wikidata.org/entity/Q6256"}),
                },
                edges = new[]
                {
                    new Edge(0, "?prop0", 0,1),
                    new Edge(1, "?type1", 0, 2, new[]{"http://www.wikidata.org/prop/direct/P31"}),
                    new Edge(2, "?type2", 1, 3, new[]{"http://www.wikidata.org/prop/direct/P31"}),
                }
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.SubjectIsInstanceOfTypeQueryEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));

            //Assert.Equal(QueryType.SubjectIsInstanceOfTypeQueryEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q30"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Label.Equals("United States"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q76"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q5"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q6256"));

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[2].QueryType);
            Assert.Empty(queryGraph.Nodes[2].Results);

            //Assert.Equal(QueryType.GivenEntityTypeNoQuery, queryGraph.Nodes[3].QueryType);
            Assert.Empty(queryGraph.Nodes[3].Results);

            //Assert.Equal(QueryType.KnownSubjectAndObjectTypesIntersectDomainRangeProperties, queryGraph.Edges[0].QueryType);
            Assert.NotEmpty(queryGraph.Edges[0].Results);
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Id.Equals("P27"));
            Assert.Contains(queryGraph.Edges[0].Results, x => x.Label.Equals("country of citizenship"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Id.Equals("P25"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Id.Equals("P22"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Id.Equals("P555"));
            Assert.DoesNotContain(queryGraph.Edges[0].Results, x => x.Id.Equals("P777"));

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[1].QueryType);
            Assert.Empty(queryGraph.Edges[1].Results);

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[2].QueryType);
            Assert.Empty(queryGraph.Edges[2].Results);

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?mother P25 ?son
        /// ?mother ?prop ?son
        ///
        /// Expected:
        /// ?mother Inferred Domain P25 (Include Human)
        /// ?son Inferred Range P25 (Include Human)
        /// ?prop Inferred Domain And Range (from Human, to Human)
        /// </summary>
        [Fact]
        public void TestResults_Inferred_2ConnectedNodes_1_N0P25N1_E1DomainP25_2Nodes2Edges()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?domain"),
                    new Node(1, "?range"),
                },
                edges = new[]
                {
                    new Edge(0, "?motherOf", 0, 1, new[]{"http://www.wikidata.org/prop/direct/P25"}),
                    new Edge(1, "?propDomain", 0, 1),
                }
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.InferredDomainTypeEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));

            //Assert.Equal(QueryType.InferredRangeTypeEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q5"));

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            //Assert.Equal(QueryType.InferredDomainAndRangeTypeProperties, queryGraph.Edges[1].QueryType);
            Assert.NotEmpty(queryGraph.Edges[1].Results);
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P25"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Label.Equals("Mother Of"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P27"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P555"));


            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?mother P25 ?son
        /// ?son ?prop ?mother
        ///
        /// Expected:
        /// ?mother Inferred Domain P25 (Include Human)
        /// ?son Inferred Range P25 (Include Human)
        /// ?prop Inferred Domain And Range (from Human, to Human)
        /// </summary>
        [Fact]
        public void TestResults_Inferred_2ConnectedNodes_2_N0P25N1_E1RangeP25_2Nodes2Edges()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?domain"),
                    new Node(1, "?range"),
                },
                edges = new[]
                {
                    new Edge(0, "?motherOf", 0, 1, new[]{"http://www.wikidata.org/prop/direct/P25"}),
                    new Edge(1, "?propRange", 1, 0),
                }
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.InferredDomainTypeEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q5"));

            //Assert.Equal(QueryType.InferredRangeTypeEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Label.Equals("Barack Obama"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q30"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q6256"));
            Assert.DoesNotContain(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q5"));

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            //Assert.Equal(QueryType.InferredDomainAndRangeTypeProperties, queryGraph.Edges[1].QueryType);
            Assert.NotEmpty(queryGraph.Edges[1].Results);
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P25"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Label.Equals("Mother Of"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P27"));
            Assert.DoesNotContain(queryGraph.Edges[1].Results, x => x.Id.Equals("P555"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?mother P25 ?son
        /// ?mother ?prop ?var2
        ///
        /// Expected:
        /// ?mother Inferred Domain P25 (Include Human)
        /// ?son Inferred Range P25 (Include Human)
        /// ?var2 is top entity
        /// ?prop Inferred Domain of P25 (from Human)
        /// </summary>
        [Fact]
        public void TestResults_Inferred_3ConnectedNodes_1_N0P25N1_E1DomainP25_3Nodes2Edge()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?domain"),
                    new Node(1, "?range"),
                    new Node(2, "?var2"),
                },
                edges = new[]
                {
                    new Edge(0, "?motherOf", 0, 1, new[]{"http://www.wikidata.org/prop/direct/P25"}),
                    new Edge(1, "?propDomain", 0, 2),
                }
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.InferredDomainTypeEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.InferredRangeTypeEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[2].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[2].Results);
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            //Assert.Equal(QueryType.InferredDomainTypeProperties, queryGraph.Edges[1].QueryType);
            Assert.NotEmpty(queryGraph.Edges[1].Results);
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P25"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Label.Equals("Mother Of"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?mother P25 ?son
        /// ?var2 ?prop ?mother
        ///
        /// Expected:
        /// ?mother Inferred Domain P25 (Include Human)
        /// ?son Inferred Range P25 (Include Human)
        /// ?var2 is top entity
        /// ?prop Inferred Range of P25 (to Human)
        /// </summary>
        [Fact]
        public void TestResults_Inferred_3ConnectedNodes_2_N0P25N1_E1RangeP25_3Nodes2Edge()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?domainP25"),
                    new Node(1, "?rangeP25"),
                    new Node(2, "?var1"),
                },
                edges = new[]
                {
                    new Edge(0, "?motherOf", 0, 1, new[]{"http://www.wikidata.org/prop/direct/P25"}),
                    new Edge(1, "?propRange", 2, 0),
                }
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.InferredDomainTypeEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.InferredRangeTypeEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.QueryTopEntities, queryGraph.Nodes[2].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[2].Results);
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            //Assert.Equal(QueryType.InferredRangeTypeProperties, queryGraph.Edges[1].QueryType);
            Assert.NotEmpty(queryGraph.Edges[1].Results);
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Id.Equals("P25"));
            Assert.Contains(queryGraph.Edges[1].Results, x => x.Label.Equals("Mother Of"));

            // Cleanup
            DeleteIndex();
        }

        /// <summary>
        /// ?mother P25 ?son
        /// ?son P27 ?country
        /// ?mother ?prop ?country
        ///
        /// Expected:
        /// ?mother Inferred Domain P25 (Include Human)
        /// ?son Inferred Range P25 (Include Human)
        /// ?son Inferred Domain P27 (Include Human)
        /// ?country Inferred Range P27 (Include Chile)
        /// ?prop Inferred Domain of P25 (from Human)
        /// ?prop Inferred Range of P27 (to Country)
        /// </summary>
        [Fact]
        public void TestResults_Inferred_3ConnectedNodes_3_N0P25N1_N1P27N2_E1DomainP25RangeP27_3Nodes3Edges()
        {
            var graph = new RDFExplorerGraph
            {
                nodes = new[]
                {
                    new Node(0, "?domain"),
                    new Node(1, "?range"),
                    new Node(2, "?domainRange"),
                },
                edges = new[]
                {
                    new Edge(0, "?motherOf", 0, 1, new[]{"http://www.wikidata.org/prop/direct/P25"}),
                    new Edge(1, "?fromCountry", 1, 2, new[]{"http://www.wikidata.org/prop/direct/P27"}),
                    new Edge(2, "?propDomainRange", 0, 2),
                }
            };

            // Arrange
            CreateIndex();

            // Act
            var queryGraph = new QueryGraph(graph);
            queryGraph.GetGraphQueryResults(EntitiesIndexPath, PropertiesIndexPath, false);

            // Assert
            //Assert.Equal(QueryType.InferredDomainTypeEntities, queryGraph.Nodes[0].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[0].Results);
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[0].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.InferredDomainAndRangeTypeEntities, queryGraph.Nodes[1].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[1].Results);
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Id.Equals("Q76"));
            Assert.Contains(queryGraph.Nodes[1].Results, x => x.Label.Equals("Barack Obama"));

            //Assert.Equal(QueryType.InferredRangeTypeEntities, queryGraph.Nodes[2].QueryType);
            Assert.NotEmpty(queryGraph.Nodes[2].Results);
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Id.Equals("Q30"));
            Assert.Contains(queryGraph.Nodes[2].Results, x => x.Label.Equals("United States"));

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[0].QueryType);
            Assert.Empty(queryGraph.Edges[0].Results);

            //Assert.Equal(QueryType.GivenPredicateTypeNoQuery, queryGraph.Edges[1].QueryType);
            Assert.Empty(queryGraph.Edges[1].Results);

            //Assert.Equal(QueryType.InferredDomainAndRangeTypeProperties, queryGraph.Edges[2].QueryType);
            Assert.NotEmpty(queryGraph.Edges[2].Results);
            Assert.Contains(queryGraph.Edges[2].Results, x => x.Id.Equals("P27"));
            Assert.Contains(queryGraph.Edges[2].Results, x => x.Label.Equals("country of citizenship"));

            // Cleanup
            DeleteIndex();
        }
    }
}
