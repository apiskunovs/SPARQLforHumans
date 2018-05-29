﻿using System.IO;
using System.Linq;
using SparqlForHumans.Core.Services;
using SparqlForHumans.Core.Utilities;
using Xunit;

namespace SparqlForHumans.UnitTests
{
    public class FilterHelperTests
    {
        [Fact]
        public void TestFilterSome()
        {
            var filename = @"TrimmedTestSet.nt";
            Assert.True(File.Exists(filename));

            var limit = 500;

            var outputFilename = FileHelper.GetFilteredOutputFilename(filename, limit);

            if (File.Exists(outputFilename))
                File.Delete(outputFilename);

            Assert.False(File.Exists(outputFilename));

            TriplesFilter.Filter(filename, outputFilename, limit);

            Assert.True(File.Exists(outputFilename));
            Assert.NotEqual(0, FileHelper.GetLineCount(outputFilename));

            var lines = FileHelper.GetInputLines(outputFilename);
            var firstItemList = lines.Select(x => x.Split(" ").FirstOrDefault());

            Assert.NotNull(firstItemList);
            Assert.NotEmpty(firstItemList);

            var qCodesList = firstItemList.Select(x => x.Split(@"<http://www.wikidata.org/entity/Q")
                                                        .LastOrDefault()
                                                        .Replace(">", ""));

            var intCodesList = qCodesList.Select(int.Parse);
            Assert.True(intCodesList.All(x => x < limit));
        }

        [Fact]
        public void TestFilterZero()
        {
            var filename = @"TrimmedTestSet.nt";
            Assert.True(File.Exists(filename));

            var limit = 0;

            var outputFilename = FileHelper.GetFilteredOutputFilename(filename, limit);

            if (File.Exists(outputFilename))
                File.Delete(outputFilename);

            Assert.False(File.Exists(outputFilename));

            TriplesFilter.Filter(filename, outputFilename, 0);

            Assert.True(File.Exists(outputFilename));
            Assert.Equal(0, FileHelper.GetLineCount(outputFilename));
        }

        [Fact]
        public void TestIsValidTriple()
        {
            //PASS: Subject: Entity-Q < 100 ; Predicate: Label ; Object: Literal
            var line = "<http://www.wikidata.org/entity/Q27> <http://www.w3.org/2000/01/rdf-schema#label> \"Ireland\"@en .";
            var triple = line.GetTriple();
            Assert.True(TriplesFilter.IsValidTriple(triple, 100));

            //PASS: Subject: Entity-Q < 100 ; Predicate: Description ; Object: Literal
            line = "<http://www.wikidata.org/entity/Q27> <http://schema.org/description> \"constitutional monarchy in Southwest Europe\"@en .";
            triple = line.GetTriple();
            Assert.True(TriplesFilter.IsValidTriple(triple, 100));

            //PASS: Subject: Entity-Q < 100 ; Predicate: Alt-Label ; Object: Literal
            line = "<http://www.wikidata.org/entity/Q27> <http://www.w3.org/2004/02/skos/core#altLabel> \"Southern Ireland\"@en .";
            triple = line.GetTriple();
            Assert.True(TriplesFilter.IsValidTriple(triple, 100));

            //PASS: Subject: Entity-P < 100
            line = "<http://www.wikidata.org/entity/P27> <http://www.w3.org/2000/01/rdf-schema#label> \"Ireland\"@en .";
            triple = line.GetTriple();
            Assert.True(TriplesFilter.IsValidTriple(triple, 100));

            //PASS: Subject: Entity-P > 100 ; 
            line = "<http://www.wikidata.org/entity/P270> <http://www.w3.org/2000/01/rdf-schema#label> \"Ireland\"@en .";
            triple = line.GetTriple();
            Assert.True(TriplesFilter.IsValidTriple(triple, 100));

            //PASS: Subject: Entity-Q < 100; Predicate: Property; Object: Entity-Q < 100; 
            line = "<http://www.wikidata.org/entity/Q27> <http://www.wikidata.org/prop/direct/P47> <http://www.wikidata.org/entity/Q26> .";
            triple = line.GetTriple();
            Assert.True(TriplesFilter.IsValidTriple(triple, 100));

            //FAIL: Subject: Entity-Q > 100 ; 
            line = "<http://www.wikidata.org/entity/Q270> <http://www.w3.org/2000/01/rdf-schema#label> \"Ireland\"@en .";
            triple = line.GetTriple();
            Assert.False(TriplesFilter.IsValidTriple(triple, 100));

            //FAIL: Subject: Entity-Q > 100; Predicate: Property; Object: Entity-Q < 100; 
            line = "<http://www.wikidata.org/entity/Q270> <http://www.wikidata.org/prop/direct/P47> <http://www.wikidata.org/entity/Q26> .";
            triple = line.GetTriple();
            Assert.False(TriplesFilter.IsValidTriple(triple, 100));

            //FAIL: Subject: Entity-Q < 100; Predicate: Property; Object: Entity-Q > 100; 
            line = "<http://www.wikidata.org/entity/Q27> <http://www.wikidata.org/prop/direct/P47> <http://www.wikidata.org/entity/Q260> .";
            triple = line.GetTriple();
            Assert.False(TriplesFilter.IsValidTriple(triple, 100));

            //FAIL: Subject: Entity-Q < 100; Predicate: Property; Object: Not Entity; 
            line = "<http://www.wikidata.org/entity/Q27> <http://www.wikidata.org/prop/direct/P47> \"Ireland\"@en .";
            triple = line.GetTriple();
            Assert.False(TriplesFilter.IsValidTriple(triple, 100));
        }
    }
}