﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SparqlForHumans.Core.Models;
using SparqlForHumans.Core.Properties;
using SparqlForHumans.Core.Utilities;
using Version = Lucene.Net.Util.Version;

namespace SparqlForHumans.Core.Services
{
    public class MultiDocumentQueries
    {
        public static IEnumerable<Entity> QueryEntitiesByLabel(string searchText, Directory luceneIndexDirectory, bool isType = false)
        {
            return QueryDocumentsByLabel(searchText, luceneIndexDirectory, isType)?.Select(x => x.MapEntity());
        }

        public static IEnumerable<Entity> QueryEntitiesByIds(IEnumerable<string> searchIds,
           Directory luceneIndexDirectory)
        {
            return QueryDocumentsByIds(searchIds, luceneIndexDirectory)?.Select(x => x.MapEntity());
        }

        public static IEnumerable<Property> QueryPropertiesByLabel(string searchText, Directory luceneIndexDirectory, bool isType = false)
        {
            return QueryDocumentsByLabel(searchText, luceneIndexDirectory, isType)?.Select(x => x.MapProperty());
        }

        public static IEnumerable<Property> QueryPropertiesByIds(IEnumerable<string> searchIds,
            Directory luceneIndexDirectory)
        {
            return QueryDocumentsByIds(searchIds, luceneIndexDirectory)?.Select(x => x.MapProperty());
        }

        public static IEnumerable<Document> QueryDocumentsByIds(IEnumerable<string> searchIds,
            Directory luceneIndexDirectory)
        {
            var documents = new List<Document>();

            // NotEmpty Validation
            if (searchIds == null)
                return documents;

            using (var searcher = new IndexSearcher(luceneIndexDirectory, true))
            using (var queryAnalyzer = new KeywordAnalyzer())
            {
                foreach (var searchText in searchIds)
                {
                    documents.Add(BaseParser.QueryDocumentByIdAndRank(searchText, queryAnalyzer, searcher));
                }

                queryAnalyzer.Close();
                searcher.Dispose();
            }

            return documents;
        }

        private static IEnumerable<Document> QueryDocumentsByLabel(string searchText, Directory luceneIndexDirectory, bool isType)
        {
            if (string.IsNullOrEmpty(searchText))
                return new List<Document>();

            searchText = BaseParser.PrepareSearchTerm(searchText);
            const int resultsLimit = 20;

            var list = new List<Document>();

            // NotEmpty Validation
            if (string.IsNullOrEmpty(searchText.Replace("*", "").Replace("?", "")))
                return list;

            Filter filter = null;
            if(isType)
                filter = new PrefixFilter(new Term(Labels.IsTypeEntity.ToString(), "true"));

            using (var searcher = new IndexSearcher(luceneIndexDirectory, true))
            using (var queryAnalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            {
                list = SearchDocuments(searchText, queryAnalyzer, searcher, resultsLimit, filter);

                queryAnalyzer.Close();
                searcher.Dispose();
            }
            return list;
        }

        //TODO: Test Search Alt-Label
        //TODO: Test Search by Id
        //TODO: UI When searching by Person shows Human but can show Person and Alt-Labels as options
        //TODO: Some instances have more than one InstanceOf
        private static List<Document> SearchDocuments(string searchText, Analyzer queryAnalyzer, Searcher searcher,
            int resultsLimit, Filter filter = null)
        {
            QueryParser parser = new MultiFieldQueryParser(Version.LUCENE_30,
                new[] { Labels.Id.ToString(), Labels.Label.ToString(), Labels.AltLabel.ToString() },
                queryAnalyzer);

            return SearchDocumentsByRank(searchText, searcher, parser, resultsLimit, filter);
        }

        private static List<Document> SearchDocumentsByRank(string searchText, Searcher searcher, QueryParser parser, int resultsLimit, Filter filter)
        {
            var sort = new Sort(SortField.FIELD_SCORE, new SortField(Labels.Rank.ToString(), SortField.DOUBLE, true));

            var query = BaseParser.ParseQuery(searchText, parser);
            var hits = searcher.Search(query, filter, resultsLimit, sort).ScoreDocs;

            return hits.Select(hit => searcher.Doc(hit.Doc)).ToList();
        }
    }
}