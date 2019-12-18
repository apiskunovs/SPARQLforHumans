﻿using System.Collections.Generic;
using System.Linq;
using SparqlForHumans.Models;
using SparqlForHumans.Models.RDFExplorer;
using SparqlForHumans.Utilities;

namespace SparqlForHumans.Lucene.Queries.Graph
{
    public class QueryNode : Node
    {
        public bool Traversed { get; set; } = false;
        public QueryNode(Node node)
        {
            id = node.id;
            name = node.name;
            uris = node.uris.Select(x => x.GetUriIdentifier()).ToArray();
        }
        public List<Entity> Results { get; set; } = new List<Entity>();
        public List<string> GivenTypes { get; set; } = new List<string>();
        public List<string> InstanceOfBaseTypes { get; set; } = new List<string>();
        public List<string> InferredBaseTypes { get; set; } = new List<string>();
        public Dictionary<string, QueryGraphExtensions.Result> Values => Results.ToDictionary();
        public bool IsGivenType { get; set; }
        public bool IsGoingToGivenType { get; set; } = false;
        public bool IsComingFromGivenType { get; set; } = false;
        public bool IsInstanceOfType { get; set; } = false;
        public bool IsInferredType => IsInferredDomainType || IsInferredRangeType;
        public bool IsInferredDomainType { get; set; } = false;
        public bool IsInferredRangeType { get; internal set; }

        public bool AvoidQuery { get; set; }

        public override string ToString()
        {
            return $"{id}:{name} ({string.Join(";", uris.Select(x => x.GetUriIdentifier()))})";
        }
    }


}
