﻿using Lucene.Net.Analysis.Core;
using SparqlForHumans.Lucene.Queries.Base;
using SparqlForHumans.Models.LuceneIndex;

namespace SparqlForHumans.Lucene.Queries.Parsers
{
    public class EntityReversePropertiesQueryParser : BaseSingleFieldQueryParser<KeywordAnalyzer>
    {
        public override KeywordAnalyzer Analyzer => new KeywordAnalyzer();
        public override Labels FieldLabel => Labels.ReverseProperty;
    }
}