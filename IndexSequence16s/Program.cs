using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IndexSequence16s
{
    class Program
    {
        // Get all sequence ID's in a line
        private static List<string> getIDs(string line)
        {
            List<string> seqIDs = new List<String>(); // array of ID's
            
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

        static void Main(string[] args)
        {
            int counter = 0; // line counter in text file
            string line; // line of text
            int position = 0; // file position of first line
            List<int> pos = new List<int>(); // an array to keep line positions in
            List<int> size = new List<int>(); // an array to keep line size in
            List<string> seqIDs = new List<string>(); // an array to keep sequence ID's from a line

            // Input file
            System.IO.StreamReader file =
                new System.IO.StreamReader(args[0]);

            // Output file
            System.IO.StreamWriter outFile =
                new System.IO.StreamWriter(args[1]);

            // Read the file and display it line by line.  
            while ((line = file.ReadLine()) != null)
            {
                seqIDs = getIDs(line);
                //System.Console.WriteLine(line); // display the line
                foreach (var seq in seqIDs)
                {
                    //Console.WriteLine(seq + " " + position);
                    outFile.WriteLine(seq + " " + position);
                }
                pos.Insert(counter, position); // store line position
                size.Insert(counter, line.Length + 1); // store line size
                counter++;
                position = position + line.Length + 1; // add 1 for '\n' character in file

            }

            file.Close();
            outFile.Close();
        }
    }
}
