using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SparqlForHumans.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparqlForHumans.Lucene.Queries.Base
{
    public abstract class BaseEntityMultiQuery : BaseEntityQuery
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();

        public BooleanQuery BooleanQuery { get; set; }

        public BaseEntityMultiQuery(string luceneIndexPath, IEnumerable<string> searchStrings, int resultsLimit = 1) : base(
    luceneIndexPath, searchStrings, resultsLimit)
        {
            this.BooleanQuery = new BooleanQuery();
        }

        public BaseEntityMultiQuery(string luceneIndexPath, string searchString, int resultsLimit = 1) : base(
            luceneIndexPath, searchString, resultsLimit)
        {
            this.BooleanQuery = new BooleanQuery();
        }

        override internal IReadOnlyList<Document> GetDocuments()
        {
            _logger.Debug("BaseEntityMultiQuery.GetDocuments() Started");

            var list = new List<Document>();

            using var luceneDirectory = FSDirectory.Open(LuceneIndexPath);
            using var luceneDirectoryReader = DirectoryReader.Open(luceneDirectory);
            var searcher = new IndexSearcher(luceneDirectoryReader);

            //var mainQuery = new BooleanQuery();

            //var searchStringQuery = new LabelsQueryParser().GetQueryParser().Parse(ParserUtilities.PrepareSearchTerm(string.Join(" ", this.SearchStrings)));
            //mainQuery.Add(searchStringQuery, Occur.MUST);

            //if (!string.IsNullOrEmpty(this._instanceOf))
            //{
            //    var instanceOfQuery = new InstanceOfQueryParser().GetQueryParser().Parse(this._instanceOf);
            //    mainQuery.Add(instanceOfQuery, Occur.MUST);
            //}

            _logger.Info($"mainQuery = {this.BooleanQuery.ToString()} ");

            var hits = searcher.Search(this.BooleanQuery, (Filter)null, ResultsLimit).ScoreDocs;
            _logger.Info($"hits [{hits.Count()}], first element={(hits.Count() > 0 ? hits[0].SerializeJsonString() : "nothing")}");

            if (hits.Count() > 0)
            {
                list.AddRange(hits.Select(hit => searcher.Doc(hit.Doc)));
                //_logger.Info($"list [{list.Count()}] = {list[0].SerializeJsonString()}");
            }

            _logger.Debug("BaseEntityMultiQuery.GetDocuments() Ended");
            return list;
        }
    }
}
