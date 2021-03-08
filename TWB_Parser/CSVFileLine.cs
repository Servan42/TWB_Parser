using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWB_Parser
{
    class CSVFileLine
    {
        #region Properties

        /// <summary>
        /// the line being read.
        /// </summary>
        private string _currentLine;
        /// <summary>
        /// The index of the current line in the file.
        /// </summary>
        private int _lineNumber;

        #endregion

        #region Attributes

        public string Event { get; private set; }
        public DateTime Date { get; private set; }
        public string Region { get; private set; }
        public int Netplay { get; private set; }
        public string Version { get; private set; }
        public string P1Name { get; private set; }
        public CharEnum P1Char1 { get; private set; }
        public CharEnum P1Char2 { get; private set; }
        public CharEnum P1Char3 { get; private set; }
        public string P2Name { get; private set; }
        public CharEnum P2Char1 { get; private set; }
        public CharEnum P2Char2 { get; private set; }
        public CharEnum P2Char3 { get; private set; }
        public string URL { get; private set; }
        /// <summary>
        /// Temporary hash that will be used to define the groupId later. 
        /// It is easier to compare this rather than the full group information.
        /// </summary>
        public int GroupHashCode { get; private set; }
        /// <summary>
        /// Used to order the matchs in the groupe.
        /// Will be set later in CSVFileData.
        /// </summary>
        public int Precedence { get; set; }
        /// <summary>
        /// Used for the correspondance between the match and the group (ie. event).
        /// Will be set later in CSVFileData.
        /// </summary>
        public string GroupId { get; set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor CSVFileLine.
        /// </summary>
        /// <param name="aCurrentLine">A TBW CSV line.</param>
        /// <param name="alineNumber">The index of the CSV line.</param>
        /// <exception cref="ArgumentException">The argument must not be null or empty.</exception>
        /// <exception cref="Exception">If the file is parsed incorrectly</exception>
        public CSVFileLine(string aCurrentLine, int alineNumber)
        {
            if (string.IsNullOrWhiteSpace(aCurrentLine)) throw new ArgumentException("Error: The CSV line lust not be null or empty.");
            this._currentLine = aCurrentLine;
            this._lineNumber = alineNumber;
            ParseCSVLine();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Load the CSV Line.
        /// <exception cref="Exception">If the file is parsed incorrectly</exception>
        /// </summary>
        private void ParseCSVLine()
        {
            try { 

                string[] splittedLine = _currentLine.Split(',');

                if (splittedLine.Length != 14)
                { 
                    throw new FormatException("The CSV line number is incomplete, it must contain 14 members.");
                }

                foreach (string member in splittedLine)
                {
                    if(String.IsNullOrWhiteSpace(member) || String.IsNullOrEmpty(member))
                    {
                        Console.WriteLine("WARNING: CSV line " + _lineNumber + " contains an empty (or whitespace) member.\nPress any key to proceed, or close the application to cancel.");
                        Console.ReadKey();
                    }
                }
                
                Event = splittedLine[0];
                Date = DateTime.Parse(splittedLine[1]);

                Region = splittedLine[2];
                if(Region != "Europe"
                    && Region != "Japan + Korea"
                    && Region != "North America"
                    && Region != "Oceania"
                    && Region != "South America") throw new FormatException("Unknown Region.");

                Netplay = Int32.Parse(splittedLine[3]);
                if (Netplay != 0 && Netplay != 1) throw new FormatException("Invalid value on the Netplay member. Must be 0 or 1.");

                Version = splittedLine[4];
                if (Version != "2E+ Final"
                    && Version != "Beowulf Patch"
                    && Version != "2E+ (old UD)"
                    && Version != "Eliza Patch"
                    && Version != "Fukua Patch"
                    && Version != "MDE"
                    && Version != "SDE"
                    && Version != "2E"
                    && Version != "Annie Patch"
                    && Version != "Annie Patch Beta") throw new FormatException("Unknown Version.");

                P1Name = splittedLine[5];
                P1Char1 = (CharEnum) Enum.Parse(typeof(CharEnum),splittedLine[6]);
                P1Char2 = (CharEnum) Enum.Parse(typeof(CharEnum),splittedLine[7]);
                P1Char3 = (CharEnum) Enum.Parse(typeof(CharEnum),splittedLine[8]);
                P2Name = splittedLine[9];
                P2Char1 = (CharEnum)Enum.Parse(typeof(CharEnum), splittedLine[10]);
                P2Char2 = (CharEnum)Enum.Parse(typeof(CharEnum), splittedLine[11]);
                P2Char3 = (CharEnum)Enum.Parse(typeof(CharEnum), splittedLine[12]);
                URL = splittedLine[13];

                // Generating a group hashcode
                StringBuilder s = new StringBuilder();
                s.Append(Event).Append(Date).Append(Region).Append(Netplay).Append(Version);
                GroupHashCode = s.ToString().GetHashCode();

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR while parsing the CSV on line " + _lineNumber + ":");
                throw e;
            }
        }

        #endregion
    }
}
