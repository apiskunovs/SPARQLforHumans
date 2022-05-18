using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SparqlForHumans.Lucene.Extensions;
using SparqlForHumans.Models;
using SparqlForHumans.Utilities;

namespace SparqlForHumans.Lucene.Queries.Base
{
    public abstract class BasePropertyMultiQuery : BasePropertyQuery
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();

        public BooleanQuery BooleanQuery { get; set; }

        public BasePropertyMultiQuery(string luceneIndexPath, IEnumerable<string> searchStrings, int resultsLimit = 1) :
            base(luceneIndexPath, searchStrings, resultsLimit)
        {
            this.BooleanQuery = new BooleanQuery();
        }

        public BasePropertyMultiQuery(string luceneIndexPath, string searchString, int resultsLimit = 1) : base(
            luceneIndexPath, searchString, resultsLimit)
        {
            this.BooleanQuery = new BooleanQuery();
        }

        override internal IReadOnlyList<Document> GetDocuments()
        {
            _logger.Debug("BasePropertyMultiQuery.GetDocuments() Started");

            var list = new List<Document>();

            using var luceneDirectory = FSDirectory.Open(LuceneIndexPath);
            using var luceneDirectoryReader = DirectoryReader.Open(luceneDirectory);
            var searcher = new IndexSearcher(luceneDirectoryReader);

            _logger.Info($"mainQuery = {this.BooleanQuery.ToString()} ");

            var hits = searcher.Search(this.BooleanQuery, (Filter)null, ResultsLimit).ScoreDocs;
            _logger.Info($"hits [{hits.Count()}], first element={(hits.Count() > 0 ? hits[0].SerializeJsonString() : "nothing")}");

            if (hits.Count() > 0)
            {
                list.AddRange(hits.Select(hit => searcher.Doc(hit.Doc)));
                //_logger.Info($"list [{list.Count()}] = {list[0].SerializeJsonString()}");
            }

            _logger.Debug("BasePropertyMultiQuery.GetDocuments() Ended");
            return list;
        }
    }
}