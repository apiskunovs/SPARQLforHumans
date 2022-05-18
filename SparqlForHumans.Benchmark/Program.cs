using CommandLine;
using SparqlForHumans.Lucene;
using System;

namespace SparqlForHumans.Benchmark
{
    class Program
    {
        private class Options
        {
            [Option('b', "basefolder",
                Required = false,
                Default = "",
                HelpText = "Base Folder where index files and directories were created (such as 'LuceneEntitiesIndex' and 'LucenePropertiesIndex')")]
            public string BaseFolder { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    LuceneDirectoryDefaults.SetBaseFolder(o.BaseFolder);

                    BenchmarkRunner.RunQueriesFromProperties();
                    BenchmarkAnalysis.LogBaseMetrics();
                    BenchmarkAnalysis.LogPointByPoint();

                    BenchmarkAnalysis.DoTheAverageRecallF1Thing();

                    //Console.ReadLine();
                    //PropertiesDistribution.CreatePropertiesDistribution();
                    Console.WriteLine("Finished");
                });
        }

        

    }
}
