using SparqlForHumans.Lucene.Queries.Base;
using SparqlForHumans.Lucene.Queries.Parsers;
using System.Collections.Generic;

namespace SparqlForHumans.Lucene.Queries.Fields
{
    public class SingleIdPropertyQuery : BasePropertyQuery
    {
        public SingleIdPropertyQuery(string luceneIndexPath, string searchString, int resultsLimit = 1) :
            base(luceneIndexPath, searchString, resultsLimit) { }

        public SingleIdPropertyQuery(string luceneIndexPath, IEnumerable<string> searchStrings, int resultsLimit = 1) :
            base(luceneIndexPath, searchStrings, resultsLimit)
        { }

        internal override IQueryParser QueryParser => new IdQueryParser();

        internal override bool IsInvalidSearchString(string inputString)
        {
            return string.IsNullOrEmpty(inputString);
        }
    }
}