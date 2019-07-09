﻿using Lucene.Net.Index;
using SparqlForHumans.RDF.Models;
using System.Collections.Generic;

namespace SparqlForHumans.Lucene.Index.Base
{
    internal interface IIndexer
    {
        IEnumerable<IFieldIndexer<IIndexableField>> RelationMappers { get; set; }
        IEnumerable<IFieldIndexer<IIndexableField>> FieldIndexers { get; set; }
        string InputFilename { get; set; }
        string OutputDirectory { get; set; }
        bool FilterGroups(SubjectGroup tripleGroup);
        void Index();
    }
}