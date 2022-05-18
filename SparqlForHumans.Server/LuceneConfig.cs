using SparqlForHumans.Lucene;
using System;

namespace SparqlForHumans.Server
{
    public class LuceneConfig
    {
        private string _entityIndexPath;
        private string _propertyIndexPath;
        public string EntityIndexPath
        {
            get { return this._entityIndexPath; }
            set { this._entityIndexPath = Environment.ExpandEnvironmentVariables(value); }
        }
        public string PropertyIndexPath
        {
            get { return this._propertyIndexPath; }
            set { this._propertyIndexPath = Environment.ExpandEnvironmentVariables(value); }
        }

        public bool InMemoryEngineEnabled { get; set; }

        public LuceneConfig()
        {
            EntityIndexPath = LuceneDirectoryDefaults.EntityIndexPath;
            PropertyIndexPath = LuceneDirectoryDefaults.PropertyIndexPath; 
            InMemoryEngineEnabled = true;
        }

        public override string ToString()
        {
            return $"EntityIndexPath={EntityIndexPath}, PropertyIndexPath={PropertyIndexPath}, InMemoryEngineEnabled={InMemoryEngineEnabled}";
        }
    }
}
