﻿using SparqlForHumans.Models;
using SparqlForHumans.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SparqlForHumans.Lucene.Queries.Graph
{

    /// Case 1:
    /// ?var0
    /// ?var1
    /// ?var0 -> ?prop -> ?var1
    /// 
    /// Case 2: P31 and prop going from the same node
    /// ?var0 -> P31 -> Qxx
    /// ?var0 -> ?prop -> ?var1
    ///
    /// Case 3: P31 going from a different node.
    /// ?var1 -> ?prop -> ?var0
    ///                   ?var0 -> P31 -> Qxx
    /// Case 4: P31 going ot from both nodes
    /// ?var0 -> P31 -> Qxx
    ///                   ?var1 -> P31 -> Qyy
    /// ?var0 -> ?prop -> ?var1
    public static class GraphQueryResults
    {
        public static void RunGraphQueryResults(this QueryGraph graph)
        {
            InMemoryQueryEngine.Init(graph.EntitiesIndexPath, graph.PropertiesIndexPath);
            //graph.RunNodeQueries(graph.EntitiesIndexPath);
            graph.RunEdgeQueries(graph.PropertiesIndexPath);
        }
        private static void RunNodeQueries(this QueryGraph graph, string indexPath)
        {
            foreach (var node in graph.Nodes.Select(x=>x.Value))
            {
                switch (node.QueryType)
                {
                    case QueryType.KnownSubjectTypeQueryInstanceEntities:
                    case QueryType.KnownSubjectAndObjectTypesQueryInstanceEntities:
                        //This should be done with the Wikipedia Endpoint
                        node.Results = new BatchIdEntityInstanceQuery(indexPath, node.Types.Select(x => x.GetUriIdentifier())).Query();
                        break;
                    case QueryType.QueryTopEntities:
                        //This should be done with the Wikipedia Endpoint
                        //node.Results = new MultiLabelEntityQuery(indexPath, "*").Query();
                        node.Results = new List<Entity>();
                        break;
                    case QueryType.InferredDomainAndRangeTypeEntities:
                    case QueryType.InferredDomainTypeEntities:
                    case QueryType.InferredRangeTypeEntities:
                        //This should be done with the Wikipedia Endpoint
                        node.Results = new BatchIdEntityInstanceQuery(indexPath, node.InferredTypes.Select(x => x.GetUriIdentifier())).Query();
                        break;
                    case QueryType.KnownPredicateAndObjectNotUsed:
                    case QueryType.KnownObjectTypeNotUsed:
                    case QueryType.ConstantTypeDoNotQuery:
                    case QueryType.Unknown:
                    default:
                        break;
                }
            }
        }

        private static void RunEdgeQueries(this QueryGraph graph, string indexPath)
        {
            foreach (var edge in graph.Edges.Select(x=>x.Value))
            {
                string targetUri = string.Empty;
                string sourceUri = string.Empty;
                //var domainProperties = new List<Property>(0);
                //var rangeProperties = new List<Property>(0);
                var domainPropertiesIds = new List<int>(0);
                var rangePropertiesIds = new List<int>(0);
                var propertiesIds = new List<string>(0);
                switch (edge.QueryType)
                {
                    case QueryType.QueryTopProperties:
                        edge.Results = new MultiLabelPropertyQuery(indexPath, "*").Query();
                        break;
                    case QueryType.KnownSubjectAndObjectTypesIntersectDomainRangeProperties:
                        domainPropertiesIds = InMemoryQueryEngine.BatchIdPropertyDomainQuery(edge.GetSourceNode(graph).Types.Select(y => y.GetUriIdentifier()));
                        rangePropertiesIds = InMemoryQueryEngine.BatchIdPropertyRangeQuery(edge.GetTargetNode(graph).Types.Select(y => y.GetUriIdentifier()));
                        propertiesIds = domainPropertiesIds.Intersect(rangePropertiesIds).Distinct().Select(x => $"P{x}").ToList();
                        edge.Results = new BatchIdPropertyQuery(indexPath, propertiesIds).Query().OrderByDescending(x => x.Rank).ToList();
                        //domainProperties = new BatchIdPropertyDomainQuery(indexPath, edge.GetSourceNode(graph).Types.Select(x => x.GetUriIdentifier())).Query();
                        //rangeProperties = new BatchIdPropertyRangeQuery(indexPath, edge.GetTargetNode(graph).Types.Select(x => x.GetUriIdentifier())).Query();
                        //edge.Results = rangeProperties.Intersect(domainProperties, new PropertyComparer()).ToList();
                        break;
                    case QueryType.KnownSubjectTypeQueryDomainProperties:
                        domainPropertiesIds = InMemoryQueryEngine.BatchIdPropertyDomainQuery(edge.GetSourceNode(graph).Types.Select(y => y.GetUriIdentifier()));
                        propertiesIds = domainPropertiesIds.Distinct().Select(x => $"P{x}").ToList();
                        edge.Results = new BatchIdPropertyQuery(indexPath, propertiesIds).Query().OrderByDescending(x => x.Rank).ToList();
                        //edge.Results = new BatchIdPropertyDomainQuery(indexPath, edge.GetSourceNode(graph).Types.Select(x => x.GetUriIdentifier())).Query();
                        break;
                    case QueryType.KnownObjectTypeQueryRangeProperties:
                        rangePropertiesIds = InMemoryQueryEngine.BatchIdPropertyRangeQuery(edge.GetTargetNode(graph).Types.Select(y => y.GetUriIdentifier()));
                        propertiesIds = rangePropertiesIds.Distinct().Select(x => $"P{x}").ToList();
                        edge.Results = new BatchIdPropertyQuery(indexPath, propertiesIds).Query().OrderByDescending(x => x.Rank).ToList();
                        //edge.Results = new BatchIdPropertyRangeQuery(indexPath, edge.GetTargetNode(graph).Types.Select(x => x.GetUriIdentifier())).Query();
                        break;
                    case QueryType.InferredDomainAndRangeTypeProperties:
                        domainPropertiesIds = InMemoryQueryEngine.BatchIdPropertyDomainQuery(edge.GetSourceNode(graph).InferredTypes.Select(y => y.GetUriIdentifier()));
                        rangePropertiesIds = InMemoryQueryEngine.BatchIdPropertyRangeQuery(edge.GetTargetNode(graph).InferredTypes.Select(y => y.GetUriIdentifier()));
                        propertiesIds = domainPropertiesIds.Intersect(rangePropertiesIds).Distinct().Select(x => $"P{x}").ToList();
                        edge.Results = new BatchIdPropertyQuery(indexPath, propertiesIds).Query();
                        //domainProperties = new BatchIdPropertyDomainQuery(indexPath, edge.GetSourceNode(graph).InferredTypes.Select(x => x.GetUriIdentifier())).Query();
                        //rangeProperties = new BatchIdPropertyRangeQuery(indexPath, edge.GetTargetNode(graph).InferredTypes.Select(x => x.GetUriIdentifier())).Query();
                        //edge.Results = rangeProperties.Intersect(domainProperties, new PropertyComparer()).ToList();
                        break;
                    case QueryType.InferredDomainTypeProperties:
                        domainPropertiesIds = InMemoryQueryEngine.BatchIdPropertyDomainQuery(edge.GetSourceNode(graph).InferredTypes.Select(y => y.GetUriIdentifier()));
                        propertiesIds = domainPropertiesIds.Distinct().Select(x => $"P{x}").ToList();
                        edge.Results = new BatchIdPropertyQuery(indexPath, propertiesIds).Query();
                        //edge.Results = new BatchIdPropertyDomainQuery(indexPath, edge.GetSourceNode(graph).InferredTypes.Select(x => x.GetUriIdentifier())).Query();
                        break;
                    case QueryType.InferredRangeTypeProperties:
                        rangePropertiesIds = InMemoryQueryEngine.BatchIdPropertyRangeQuery(edge.GetTargetNode(graph).InferredTypes.Select(y => y.GetUriIdentifier()));
                        propertiesIds = rangePropertiesIds.Distinct().Select(x => $"P{x}").ToList();
                        edge.Results = new BatchIdPropertyQuery(indexPath, propertiesIds).Query();
                        //edge.Results = new BatchIdPropertyRangeQuery(indexPath, edge.GetTargetNode(graph).InferredTypes.Select(x => x.GetUriIdentifier())).Query();
                        break;
                    case QueryType.ConstantTypeDoNotQuery:
                    case QueryType.Unknown:
                    default:
                        break;
                }
            }
        }
    }
}
