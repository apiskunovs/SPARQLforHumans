using Lucene.Net.Documents;
using Lucene.Net.Search;
using SparqlForHumans.Lucene.Queries.Base;
using SparqlForHumans.Lucene.Queries.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparqlForHumans.Lucene.Queries.Fields
{
    public class FindPropertyAroundPropertyQuery : BasePropertyQuery
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();
        private string _luceneEntityIndexPath;
        private string _lucenePropertyIndexPath;
        private FindMode _findMode;
        public enum FindMode
        {
            NextProperties,
            PreviousProperties
        }

        public FindPropertyAroundPropertyQuery(string luceneEntityIndexPath, string lucenePropertyIndexPath, string searchString, FindMode findMode, int resultsLimit = 20)
            : base(luceneEntityIndexPath, searchString, resultsLimit)
        {
            this._luceneEntityIndexPath = luceneEntityIndexPath;
            this._lucenePropertyIndexPath = lucenePropertyIndexPath;
            this._findMode = findMode;
        }

        // specified but in practice not used, because BaseEntityMultiQuery constist of several queries and each can have its own parser 
        // building query happens in GetDocuments() function
        internal override IQueryParser QueryParser => new LabelsQueryParser(); 

        override internal IReadOnlyList<Document> GetDocuments()
        {
            // retrieve instances which property definition
            var searchString = string.Join(" ", this.SearchStrings).Trim();
            var filteredItems = new FindInstancesQuery(
                this._luceneEntityIndexPath, 
                (null as string), 
                null,
                (this._findMode == FindMode.NextProperties? searchString : null),
                (this._findMode == FindMode.PreviousProperties ? searchString : null),
                null,
                null,
                null,
                20000
            ).Query(this.ResultsLimit);

            // build unique set of properties
            HashSet<string> hashSet = new HashSet<string>();
            foreach (var entity in filteredItems)
            {
                switch (this._findMode)
                {
                    case FindMode.NextProperties:
                        hashSet.UnionWith(entity.Properties.Select(x => x.Id));
                        break;

                    case FindMode.PreviousProperties:
                        hashSet.UnionWith(entity.ReverseProperties.Select(x => x.Id));
                        break;

                    default:
                        throw new NotImplementedException($"FindMode {this._findMode.ToString()} not implemented");
                }
            }

            var query = new SingleIdPropertyQuery(this._lucenePropertyIndexPath, string.Join(' ', hashSet), this.ResultsLimit);
            return query.GetDocuments();
        }
    }
}
