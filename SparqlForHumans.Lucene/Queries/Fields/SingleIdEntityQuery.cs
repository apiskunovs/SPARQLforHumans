using SparqlForHumans.Lucene.Queries.Base;
using SparqlForHumans.Lucene.Queries.Parsers;

namespace SparqlForHumans.Lucene.Queries.Fields
{
    public class SingleIdEntityQuery : BaseEntityQuery
    {
        public SingleIdEntityQuery(string luceneIndexPath, string searchString, int resultsLimit = 1) :
            base(luceneIndexPath, searchString, resultsLimit) { }

        internal override IQueryParser QueryParser => new IdQueryParser();

        internal override bool IsInvalidSearchString(string inputString)
        {
            return string.IsNullOrEmpty(inputString);
        }
    }
}