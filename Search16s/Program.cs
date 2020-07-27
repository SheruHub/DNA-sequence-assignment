using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Search16s
{
    // ===============================
    // Search16s
    //
    // AUTHOR : Sheru Bharadwaja
    // CREATE DATE : 20 October 2019
    // PURPOSE : 
    //  Conduct various fasta file searches
    //  based on command-line arguments 
    // ===============================
    class Search16s
    {

        static void Main(string[] args)
        {
            try
            {
                argHandler ah = new argHandler(args); // Process command line arguments
                SearchProcessor sp = new SearchProcessor(ah.SearchVars["fastaFile"]); // Pass args to search processor

                switch (ah.SearchType)
                {
                    case 1:
                        sp.level1(int.Parse(ah.SearchVars["startPos"]), int.Parse(ah.SearchVars["numSeq"]));
                        break;
                    case 2:
                        sp.level2(ah.SearchVars["seqId"]);
                        break;
                    case 3:
                        sp.level3(ah.SearchVars["queryFile"], ah.SearchVars["resultsFile"]);
                        break;
                    case 4:
                        sp.level4(ah.SearchVars["indexFile"], ah.SearchVars["queryFile"], ah.SearchVars["resultsFile"]);
                        break;
                    case 5:
                        sp.level5(ah.SearchVars["sequence"]);
                        break;
                    case 6:
                        sp.level6(ah.SearchVars["word"]);
                        break;
                    case 7:
                        sp.level7(ah.SearchVars["searchString"]);
                        break;
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine(e.Message);
            }
        }
    }
}