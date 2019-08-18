using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWB_Parser
{
    class CSVFileData
    {
        #region Constants

        /// <summary>
        /// Header of the CSV File.
        /// </summary>
        private const string _CSVHEADER = "Event,Date,Region,Netplay,Version,P1Name,P1Char1,P1Char2,P1Char3,P2Name,P2Char1,P2Char2,P2Char3,URL";

        #endregion

        #region Attributes

        /// <summary>
        /// Each case of this list is a line of the CSV file.
        /// </summary>
        public List<CSVFileLine> Lines { get; private set; }
        /// <summary>
        /// Path to the CSV file.
        /// </summary>
        public string Path { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of CSVFileData.
        /// </summary>
        /// <param name="aPath">The path of the CSV file to load.</param>
        /// <exception cref="ArgumentNullException">The path must not be null.</exception>
        /// <exception cref="FileNotFoundException">In case the file is missing.</exception>
        /// <exception cref="Exception">If the file is parsed incorrectly</exception>
        public CSVFileData(string aPath)
        {
            if (string.IsNullOrWhiteSpace(aPath)) throw new ArgumentNullException("aPath");
            if (!File.Exists(aPath)) throw new FileNotFoundException("ERROR: The CSV file does not exist.", aPath);
            this.Lines = new List<CSVFileLine>();
            this.Path = aPath;
            // Loading the file
            ParseCSVFile();
            // Creating the groupIds and the setting up the precedence for each match.
            GenerateGroupIdAndPrecedence();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Parses the CSV file and load the lines.
        /// </summary>
        /// <exception cref="FormatException">If the file is not a TWB CSV file.</exception>
        /// <exception cref="Exception">If the file is parsed incorrectly</exception>
        private void ParseCSVFile()
        {
            using (StreamReader sr = new StreamReader(Path))
            {
                string currentLine;
                int lineNumber = 1;
                currentLine = sr.ReadLine();
                // Checking if the file is a TWB CSV file.
                if (currentLine != null && !currentLine.Contains(_CSVHEADER))
                {
                    throw new FormatException("ERROR: The file is not a TWB CSV file. Invalid header.");
                }
                // Reading the lines and adding them to the list.
                while ((currentLine = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    CSVFileLine CSVLine = new CSVFileLine(currentLine,lineNumber);
                    Lines.Add(CSVLine);
                }
            }
        }

        /// <summary>
        /// After the lines have been loaded into "Lines", complete the information
        /// with the groupId and the precedence of the match.
        /// <requires>this.ParseCSVFile() must have been called before.</requires>
        /// </summary>
        private void GenerateGroupIdAndPrecedence()
        {
            int groupeHashCode = 0;
            int precedence = 1;
            string groupeId = "Not_Initialized";

            foreach (CSVFileLine line in Lines)
            {
                // If the match belongs to a new group, create the groupe, else, increment the precedence.
                if(line.GroupHashCode != groupeHashCode)
                {
                    groupeHashCode = line.GroupHashCode;
                    groupeId = Guid.NewGuid().ToString();
                    precedence = 1;
                }
                else
                {
                    precedence++;
                }

                line.GroupId = groupeId;
                line.Precedence = precedence;
            }
        }

        #endregion
    }
}
