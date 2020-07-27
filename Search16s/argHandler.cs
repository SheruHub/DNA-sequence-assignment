using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search16s
{
    // ===============================
    // argHandler
    //
    // AUTHOR : Sheru Bharadwaja
    // CREATE DATE : 20 October 2019
    // PURPOSE : 
    //  Takes command-line arguments and error-checks them,
    //  and if valid, converts them to usable variables
    // ===============================
    class argHandler
    {
        private string[] args;

        // hold meaningful arguments
        private Dictionary<string, string> searchVars = new Dictionary<string, string>();
        public Dictionary<string, string> SearchVars
        {
            get
            {
                return searchVars;
            }
        }

        private int searchType;
        public int SearchType
        {
            get
            {
                return searchType;
            }
        }

        //
        // Summary:
        //     Initializes a new instance argHandler.
        //
        // Parameters:
        //   args:
        //     The command line argument array.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Syntax and/or format of command line arguments is incorrect.
        //
        //   T:System.ArgumentNullException:
        //     No arguments given.
        public argHandler(string[] args)
        {
            if (args.Length == 0) throw new System.ArgumentException("Error: No arguments given");
            this.args = args;
            this.searchType = findSearchType();
            checkArgs();
        }


        /**
        * Check command line arguments for errors
        * Returns true if valid input string. False if not
        * */
        public bool checkArgs()
        {
            int argCount = this.args.Length;

            switch (this.searchType)
            {
                // Level 1 search string
                // Search16s -level1 filename.fasta 273 10 
                case 1:
                    if (argCount == 4)
                    {
                        // Make sure starting position and num sequences are positive integers
                        if (int.TryParse(this.args[2], out int arg2) && int.TryParse(this.args[3], out int arg3) && arg2 > 0 && arg3 > 0)
                        {
                            // Make sure starting position is an odd number
                            if (arg2 % 2 != 0)
                            {
                                searchVars["fastaFile"] = args[1];
                                searchVars["startPos"] = args[2];
                                searchVars["numSeq"] = args[3];
                                return true;
                            }
                            throw new System.ArgumentException("Starting line number must be odd number");
                        }
                        throw new System.ArgumentException("Start line number and number of sequences must be positive integers");

                    }
                    throw new System.ArgumentException("Invalid command line syntax for level 1 search\nFormat: Search16s -level1 <filename.fasta> <start position> <num lines>");

                // Level 2 search string
                // Search16s -level2 16S.fasta NR_115365.1
                case 2:
                    if (argCount == 3)
                    {
                        searchVars["fastaFile"] = args[1];
                        searchVars["seqId"] = args[2];
                        return true;
                    }
                    throw new System.ArgumentException("Invalid command line syntax for level 2 search\nFormat: Search16s -level2 <filename.fasta> <sequence ID>");

                // Level 3 search string
                // Search16s -level3 16S.fasta query.txt results.txt
                case 3:
                    if (argCount == 4)
                    {
                        searchVars["fastaFile"] = args[1];
                        searchVars["queryFile"] = args[2];
                        searchVars["resultsFile"] = args[3];
                        return true;
                    }
                    throw new System.ArgumentException("Invalid command line syntax for level 3 search\nFormat: Search16s -level3 <filename.fasta> <query file> <output file>");

                // Level 4 serch string
                // Search16s -level4 16S.fasta 16S.index query.txt results.txt
                case 4:
                    if (argCount == 5)
                    {
                        searchVars["fastaFile"] = args[1];
                        searchVars["indexFile"] = args[2];
                        searchVars["queryFile"] = args[3];
                        searchVars["resultsFile"] = args[4];
                        return true;
                    }
                    throw new System.ArgumentException("Invalid command line syntax for level 4 search\nFormat: Search16s -level4 <filename.fasta> <index file> <query file> <results file>");

                // Level 5 search string
                // Search16s -level5 16S.fasta CTGGTACGGTCAACTTGCTCTAAG
                case 5:
                    if (argCount == 3)
                    {
                        searchVars["fastaFile"] = args[1];
                        searchVars["sequence"] = args[2];
                        return true;
                    }
                    throw new System.ArgumentException("Invalid command line syntax for level 5 search\nFormat: Search16s -level5 <filename.fasta> <sequence>");

                // Level 6 search string
                // Search16s -level6 16S.fasta Streptomyces
                case 6:
                    if (argCount == 3)
                    {
                        searchVars["fastaFile"] = args[1];
                        searchVars["word"] = args[2];
                        return true;
                    }
                    throw new System.ArgumentException("Invalid command line syntax for level 6 search\nFormat: Search16s -level6 <filename.fasta> <word>");

                // Level 7 search string
                // Search16s -level7 16S.fasta ACTG*GTAC*CA
                case 7:
                    if (argCount == 3)
                    {
                        SearchVars["fastaFile"] = args[1];
                        searchVars["searchString"] = args[2];
                        return true;
                    }
                    throw new System.ArgumentException("Invalid command line syntax for level 7 search\nFormat: Search16s -level7 <filename.fasta> <search string>");

                default:
                    throw new System.ArgumentException("Invalid search-level flag");
            }

        }


        /**
         * Finds the search level (type) from the command line args
         * returns an int of the type if found, otherwise throws an error
         * */
        public int findSearchType()
        {
            string test = "-LEVEL";
            string compare;

            if (this.args[0] == null) throw new System.ArgumentNullException("level"); // Make sure level argument exists
            else compare = this.args[0].ToUpper();

            for (int i = 0; i < test.Length; i++) // Make sure arg0 starts with "-LEVEL"
            {
                if (test[i] != compare[i]) throw new System.ArgumentException("Incorrect search level syntax. Correct format: -level<n>", "level");
            }

            string numString = "";
            for (int i = test.Length; i < compare.Length; i++)
            {
                numString += compare[i]; // Get the number after "-LEVEL"
            }

            return Int32.Parse(numString); // return search type
        }
    }
}
