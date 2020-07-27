using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Search16s
{

    // ===============================
    // SearchProcessor
    //
    // AUTHOR : Sheru Bharadwaja
    // CREATE DATE : 20 October 2019
    // PURPOSE : Processes searches level 1-7
    // ===============================
    class SearchProcessor
    {

        private StreamReader fastaFile;

        public SearchProcessor(string file)
        {
            fastaFile = getFile(file);
        }

        /**
         * Checks for a valid sequence ID line when given a string
         * returns true if valid sequence ID line, false if not
         * */
        private static bool lineValid(string line)
        {
            bool valid=false;
            try
            {
                valid = (line[0] == '>') ? true : false;
            }
            catch (System.IndexOutOfRangeException e)
            {
                // Set IndexOutOfRangeException to the new exception's InnerException.
                throw new System.ArgumentOutOfRangeException("Sequence id delimiter character not found. Check file format.", e);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            return valid;
        }

        // Get all sequence ids from meta data line
        private static List<string> getIDs(string line)
        {
            List<string> seqIDs = new List<String>(); // array of ID's

            if (!lineValid(line)) throw new Exception("Error: Sequence meta-data line invalid");

            string[] fullSeq = line.Split('>'); // separate line into sequence info

            if (line[0] == '>') // Only lines that start with an ID
            {

                foreach (var seq in fullSeq)
                {
                    string[] id = seq.Split(' '); // seperate sequence info into words
                    if (id[0].Length > 0) seqIDs.Add(id[0]); // get only the ID
                }
            }

            return seqIDs;
        }

        /**
         * Tries to open a file given the filename as a string.
         * Returns a new StreamReader of the file if successful.
         * Returns null and prints error if file couldn't be opened
         * */
        private static StreamReader getFile(string fileName)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                return new StreamReader(fileName);
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            throw new System.Exception("Error loading file: " + fileName);

        }


        /**
         * Load the index file into a usable Dictionary
         * */
        private Dictionary<string, int> loadIndex(StreamReader indexFile)
        {
            Dictionary<string, int> index = new Dictionary<string, int>();

            using (indexFile)
            {
                // read through the index file
                while (!indexFile.EndOfStream)
                {
                    string[] line = indexFile.ReadLine().Split(' '); // read line parts
                    index[line[0]] = int.Parse(line[1]); // load into dictionary
                }
            }
            return index;
        }

        /**
         * Conduct a level1 search
         * */
        public void level1(int startPos, int numSequences)
        {
            // The using statement closes the StreamReader.
            using (fastaFile)
            {
                // Get to starting position
                for (int i = 1; i < startPos; i++)
                {
                    if (fastaFile.EndOfStream)
                    {
                        Console.WriteLine("Error: start position is greater than number of lines in file");
                        return;
                    }
                    fastaFile.ReadLine();
                }
                // Print requested sequences
                for (int i = 0; i < numSequences; i++)
                {
                    // Check for end of file before sequence data line read
                    if (fastaFile.EndOfStream)
                    {
                        Console.WriteLine("Error: not enough lines in fasta file to complete request");
                        return;
                    }
                    string dataLine = fastaFile.ReadLine(); // Get data line

                    // Another end of file check before sequence line read
                    if (fastaFile.EndOfStream)
                    {
                        Console.WriteLine("Error: not enough lines in fasta file to complete request");
                        return;
                    }
                    string seqLine = fastaFile.ReadLine(); // Get sequence line

                    // check that first line starts with '>', and second line doesn't
                    if (lineValid(dataLine) && !lineValid(seqLine))
                    {
                        Console.WriteLine(dataLine);
                        Console.WriteLine(seqLine);
                    }
                    else
                    {
                        Console.WriteLine("Error: fasta file format invalid");
                        return;
                    }

                }
            }
        }

        /**
         * Conduct a level2 search
         * */
        public void level2(string seqId)
        {

            bool found = false;
            string seqInfo;
            string seq;

            using (fastaFile)
            {
                while (!fastaFile.EndOfStream)
                {
                    seqInfo = fastaFile.ReadLine();
                    seq = fastaFile.ReadLine();

                    if (seqInfo.Contains(seqId.ToUpper()))
                    {
                        if (found == false) found = true;
                        Console.WriteLine(seqInfo);
                        Console.WriteLine(seq);
                    }
                }
                if (!found) Console.WriteLine("Error: sequence " + seqId + " not found");
            }
        }

        /**
         * Conduct a level3 search
         * */
        public void level3(string _queryFile, string _outFile)
        {
            StreamReader queryFile = getFile(_queryFile); // Query file
            StreamWriter outFile = new StreamWriter(_outFile); // Output file


            using (queryFile)
            using (fastaFile)
            using (outFile)
            {
                // get each line of the query file
                while (!queryFile.EndOfStream)
                {
                    string query = queryFile.ReadLine();
                    bool found = false;

                    // search the fasta file
                    while (!fastaFile.EndOfStream)
                    {
                        string seqInfo = fastaFile.ReadLine();
                        string seq = fastaFile.ReadLine();

                        if (seqInfo.Contains(query.ToUpper()))
                        {
                            if (found == false) found = true;
                            outFile.WriteLine(seqInfo);
                            outFile.WriteLine(seq);
                        }
                    }
                    // Return file stream back to start of fasta file
                    fastaFile.DiscardBufferedData();
                    fastaFile.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

                    if (!found) Console.WriteLine("Error: sequence " + query + " not found");
                }
            }
        }

        /**
         * Conduct a level4 search
         * */
        public void level4(string _indexFile, string _queryFile, string _resultsFile)
        {
            StreamReader indexFile = getFile(_indexFile); // Index file
            StreamReader queryFile = getFile(_queryFile); // Query file
            StreamWriter resultsFile = new StreamWriter(_resultsFile); // Results file

            Dictionary<string, int> index = loadIndex(indexFile);

            using (fastaFile)
            using (queryFile)
            using (resultsFile)
            {
                // get each line of the query file                
                while (!queryFile.EndOfStream)
                {
                    string query = queryFile.ReadLine();

                    // Check if id exists in index
                    if (index.ContainsKey(query))
                    {
                        // Move to location of index'd id within fasta file
                        fastaFile.DiscardBufferedData();
                        fastaFile.BaseStream.Seek(index[query], System.IO.SeekOrigin.Begin);

                        // Write results to output file
                        resultsFile.WriteLine(fastaFile.ReadLine());
                        resultsFile.WriteLine(fastaFile.ReadLine());
                    }
                    else Console.WriteLine("Error: sequence {0} not found", query);
                }
            }
        }

        /**
         * Conduct a level5 search
         * */
        public void level5(string sequence)
        {

            bool found = false;
            string seqInfo;
            string seq;
            List<string> ids = new List<string>();

            using (fastaFile)
            {
                while (!fastaFile.EndOfStream)
                {
                    // Read sequence info line and make sure it's valid
                    seqInfo = fastaFile.ReadLine();
                    if (!lineValid(seqInfo))
                    {
                        throw new Exception("Fasta file format invalid");
                    }
                    seq = fastaFile.ReadLine();

                    // Match found
                    if (seq.Contains(sequence))
                    {
                        if (!found) found = true;
                        ids = getIDs(seqInfo);
                        foreach (var id in ids)
                        {
                            Console.WriteLine(id);
                        }
                    }
                }
                if (!found) Console.WriteLine("Error: sequence {0} not found", sequence);
            }
        }

        /**
         * Conduct a level6 search
         * */
        public void level6(string word)
        {

            bool found = false;
            string seqInfo;
            List<string> ids = new List<string>();

            using (fastaFile)
            {
                while (!fastaFile.EndOfStream)
                {
                    // Read sequence info line and make sure it's valid
                    seqInfo = fastaFile.ReadLine();
                    fastaFile.ReadLine();
                    if (!lineValid(seqInfo))
                    {
                        throw new Exception("Fasta file format invalid");
                    }

                    // Match found
                    if (seqInfo.Contains(word))
                    {
                        if (!found) found = true;
                        ids = getIDs(seqInfo);
                        foreach (var id in ids)
                        {
                            Console.WriteLine(id);
                        }
                    }
                }
                if (!found) Console.WriteLine("Error: word '{0}' not found", word);
            }
        }

        /**
         * Conduct a level7 search
         * */
        public void level7(string searchString)
        {

            bool found = false;
            string seqInfo;
            string seq;
            int total = 0;


            // Define a regular expression from input string.
            string rxString = Regex.Replace(searchString, "[*]", "(.*)");
            Regex rx = new Regex(@rxString);

            MatchCollection matches;

            using (fastaFile)
            {
                while (!fastaFile.EndOfStream)
                {

                    // Read sequence info line and make sure it's valid
                    seqInfo = fastaFile.ReadLine();
                    seq = fastaFile.ReadLine();
                    if (!lineValid(seqInfo))
                    {
                        throw new Exception("Fasta file format invalid");
                    }

                    // Find matches.
                    matches = rx.Matches(seq);

                    if (matches.Count > 0)
                    {
                        if (!found) found = true;
                        // Get all id's in matched sequence id line
                        List<string> ids = getIDs(seqInfo);
                        foreach (var id in ids)
                        {
                            total++;
                            Console.WriteLine("Match found in: {0}", id);
                        }
                    }

                }
                if (!found) Console.WriteLine("Error: word '{0}' not found", searchString);
            }
            Console.WriteLine("Total matches found: {0}\nSearch string used:\n{1}", total, searchString);
        }
    }
}
