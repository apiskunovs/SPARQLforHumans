using Lucene.Net.Documents;
using Lucene.Net.Search;
using SparqlForHumans.Lucene.Queries.Base;
using SparqlForHumans.Lucene.Queries.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SparqlForHumans.Lucene.Queries.Fields
{
    public class FindPropertiesQuery : BasePropertyMultiQuery
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();

        private string _domain = null;
        private string _range = null;
        private string _id = null;
        private string _idNot = null;

        public FindPropertiesQuery(string luceneIndexPath, string searchString, 
            string domain, string range, string id, string idNot,
            int resultsLimit = 20) : base(
    luceneIndexPath, searchString, resultsLimit)
        {
            this._domain = domain;
            this._range = range;
            this._id = id;
            this._idNot = idNot;
        }

        // specified but in practice not used, because BaseEntityMultiQuery constist of several queries and each can have its own parser 
        // building query happens in GetDocuments() function
        internal override IQueryParser QueryParser => new LabelsQueryParser(); 

        override internal IReadOnlyList<Document> GetDocuments()
        {
            // build Boolean query of multiple other queries
            string tmp = string.Join(" ", this.SearchStrings).Trim();
            if (!string.IsNullOrEmpty(tmp) && tmp != "*")
            {
                var searchStringQuery = new LabelsQueryParser().GetQueryParser().Parse(ParserUtilities.PrepareSearchTerm(tmp));
                this.BooleanQuery.Add(searchStringQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(this._range))
            {
                var rangeQuery = new RangeQueryParser().GetQueryParser().Parse(this._range);
                this.BooleanQuery.Add(rangeQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(this._domain))
            {
                var domainQuery = new DomainQueryParser().GetQueryParser().Parse(this._domain);
                this.BooleanQuery.Add(domainQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(this._id))
            {
                var idQuery = new IdQueryParser().GetQueryParser().Parse(this._id);
                this.BooleanQuery.Add(idQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(this._idNot))
            {
                var idNotQuery = new IdQueryParser().GetQueryParser().Parse(this._idNot);
                this.BooleanQuery.Add(idNotQuery, Occur.MUST_NOT);
            }

            _logger.Info($"do base.GetDocuments()");
            // initiate base GetDocuments() from BaseEntityMultiQuery
            return base.GetDocuments();
        }
    }
}
