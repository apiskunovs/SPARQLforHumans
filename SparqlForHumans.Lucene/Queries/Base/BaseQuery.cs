using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SparqlForHumans.Lucene.Queries.Parsers;
using SparqlForHumans.Models;
using SparqlForHumans.Utilities;

namespace SparqlForHumans.Lucene.Queries.Base
{
    public abstract class BaseQuery<T> : IQuery
        where T : Subject
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();

        public BaseQuery(string luceneIndexPath, IEnumerable<string> searchStrings, int resultsLimit = 1)
        {
            LuceneIndexPath = luceneIndexPath;
            SearchStrings = searchStrings;
            ResultsLimit = resultsLimit;
            queryParser = QueryParser.GetQueryParser();
        }

        public BaseQuery(string luceneIndexPath, string searchString, int resultsLimit = 1)
        {
            LuceneIndexPath = luceneIndexPath;
            SearchStrings = new List<string> {searchString};
            ResultsLimit = resultsLimit;
            queryParser = QueryParser.GetQueryParser();
        }

        internal virtual Filter Filter { get; set; } = null;
        internal QueryParser queryParser { get; }
        internal abstract IQueryParser QueryParser { get; }
        internal int ResultsLimit { get; set; }
        internal IEnumerable<string> SearchStrings { get; set; }
        public string LuceneIndexPath { get; set; }
        public abstract IEnumerable<T> Query(int totalResultsLimit = 100);

        internal virtual IReadOnlyList<Document> GetDocuments()
        {
            _logger.Debug("GetDocuments Start");
            var list = new List<Document>();

            _logger.Debug($"Search strings: {string.Join(',', SearchStrings)}");
            if (SearchStrings.All(IsInvalidSearchString))
                return list;

            _logger.Debug($"LuceneIndexPath = {LuceneIndexPath} ");
            using var luceneDirectory = FSDirectory.Open(LuceneIndexPath);
            using var luceneDirectoryReader = DirectoryReader.Open(luceneDirectory);
            var searcher = new IndexSearcher(luceneDirectoryReader);

            foreach (var searchString in SearchStrings) {
                if (IsInvalidSearchString(searchString)) continue;
                var preparedSearchTerm = PrepareSearchTerm(searchString);
                _logger.Debug($"preparedSearchTerm = {preparedSearchTerm} ");

                var query = ParserUtilities.ParseQuery(preparedSearchTerm, queryParser);
                _logger.Info($"query = {query.ToString()} ");

                var hits = searcher.Search(query, Filter, ResultsLimit).ScoreDocs;
                _logger.Debug($"hits [{hits.Count()}] = {hits.SerializeJsonString()}");

                list.AddRange(hits.Select(hit => searcher.Doc(hit.Doc)));
                _logger.Debug($"list [{list.Count()}]: {(list.Count > 0 ? list[0].SerializeJsonString() : "")}");
            }

            _logger.Info($"GetDocuments End [{list.Count()}]");

            return list;
        }

        internal virtual bool IsInvalidSearchString(string inputString)
        {
            return false;
        }

        internal virtual string PrepareSearchTerm(string inputString)
        {
            return inputString.GetUriIdentifier();
        }
    }
}