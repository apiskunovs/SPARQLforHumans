﻿using SparqlForHumans.Lucene.Queries.Base;
using SparqlForHumans.Lucene.Queries.Parsers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SparqlForHumans.Lucene.Queries
{
    public class BatchIdPropertyDomainQuery : BasePropertyQuery
    {
        public BatchIdPropertyDomainQuery(string luceneIndexPath, IEnumerable<string> searchStrings) : base(luceneIndexPath, searchStrings) { }

        internal override IQueryParser QueryParser => new DomainQueryParser();
        internal override bool IsInvalidSearchString(string inputString) => string.IsNullOrEmpty(Regex.Replace(inputString, @"[\D]", string.Empty));
        internal override string PrepareSearchTerm(string inputString) => Regex.Replace(inputString, @"[\D]", string.Empty);
    }
}
