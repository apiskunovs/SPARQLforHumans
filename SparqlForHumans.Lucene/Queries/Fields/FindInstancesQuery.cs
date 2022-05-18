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
    public class FindInstancesQuery : BaseEntityMultiQuery
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();

        private string _instanceOf = null;
        private string _incomingProperty = null;
        private string _outgoingProperty = null;
        private string _id = null;
        private string _idNot = null;
        private string _isType = null;

        public FindInstancesQuery(string luceneIndexPath, string searchString, string instanceOf
            , string incomingProperty, string outgoingProperty, string id, string idNot, string isType
            , int resultsLimit = 20) : base(
    luceneIndexPath, searchString, resultsLimit)
        {
            this._instanceOf = instanceOf;
            this._incomingProperty = incomingProperty;
            this._outgoingProperty = outgoingProperty;
            this._id = id;
            this._idNot = idNot;
            this._isType = isType;
        }

        public FindInstancesQuery(string luceneIndexPath, IEnumerable<string> searchStrings, string instanceOf
            , string incomingProperty, string outgoingProperty, string id, string idNot, string isType
            , int resultsLimit = 20) : base(
            luceneIndexPath, searchStrings, resultsLimit)
        {
            this._instanceOf = instanceOf;
            this._incomingProperty = incomingProperty;
            this._outgoingProperty = outgoingProperty;
            this._id = id;
            this._idNot = idNot;
            this._isType = isType;
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

            if (!string.IsNullOrEmpty(this._instanceOf))
            {
                var instanceOfQuery = new InstanceOfQueryParser().GetQueryParser().Parse(this._instanceOf);
                this.BooleanQuery.Add(instanceOfQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(this._outgoingProperty))
            {
                var entityPropertiesQuery = new EntityPropertiesQueryParser().GetQueryParser().Parse(this._outgoingProperty);
                this.BooleanQuery.Add(entityPropertiesQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(this._incomingProperty))
            {
                var entityReversePropertiesQuery = new EntityReversePropertiesQueryParser().GetQueryParser().Parse(this._incomingProperty);
                this.BooleanQuery.Add(entityReversePropertiesQuery, Occur.MUST);
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

            if (!string.IsNullOrEmpty(this._isType))
            {
                bool result;
                bool success = bool.TryParse(this._isType, out result);
                if (success)
                {
                    var isTypeQuery = new IsTypeQueryParser().GetQueryParser().Parse(true.ToString());
                    if (result)
                    {
                        this.BooleanQuery.Add(isTypeQuery, Occur.MUST);
                    }
                    else
                    {
                        if (this.BooleanQuery.Clauses.Count == 0)
                            this.BooleanQuery.Add(new MatchAllDocsQuery(), Occur.SHOULD);

                        this.BooleanQuery.Add(isTypeQuery, Occur.MUST_NOT);
                    }
                }

                //// attempt to get data without isType fields
                //var str = this._isType;
                //var neg = (str[0] == '$');
                //if (neg) str = str.Substring(1);

                //var isTypeQuery = new IsTypeQueryParser().GetQueryParser().Parse(str);
                //this.BooleanQuery.Add(isTypeQuery, (neg ? Occur.MUST_NOT : Occur.MUST));
            }
            // initiate base GetDocuments() from BaseEntityMultiQuery
   
            return base.GetDocuments();
        }
    }
}
