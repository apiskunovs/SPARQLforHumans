using SparqlForHumans.Lucene;
using System;

namespace SparqlForHumans.Server
{
    public class LuceneConfig
    {
        public string BaseFolder
        {
            get { return LuceneDirectoryDefaults.BaseFolder; }
            set { LuceneDirectoryDefaults.SetBaseFolder(value); }
        }
        public string EntityIndexPath
        {
            get { return LuceneDirectoryDefaults.EntityIndexPath; }
        }
        public string PropertyIndexPath
        {
            get { return LuceneDirectoryDefaults.PropertyIndexPath; }
        }

        public bool InMemoryEngineEnabled { get; set; }

        public LuceneConfig()
        {
            InMemoryEngineEnabled = true;
        }

        public override string ToString()
        {
            return $"EntityIndexPath={EntityIndexPath}, PropertyIndexPath={PropertyIndexPath}, InMemoryEngineEnabled={InMemoryEngineEnabled}";
        }
    }
}
