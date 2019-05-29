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
        /// <exception cref="ArgumentException">The argument must not be null or empty.</exception>
        public CSVFileLine(string aCurrentLine)
        {
            if (string.IsNullOrWhiteSpace(aCurrentLine)) throw new ArgumentException("Error: The CSV line lust not be null or empty.");
            this._currentLine = aCurrentLine;
            ParseCSVLine();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Load the CSV Line.
        /// </summary>
        private void ParseCSVLine()
        {
            string[] splittedLine = _currentLine.Split(',');

            if (splittedLine.Length != 14)
                throw new FormatException("Error: The CSV line is incomplete.");

            Event = splittedLine[0];
            Date = DateTime.Parse(splittedLine[1]);
            Region = splittedLine[2];
            Netplay = Int32.Parse(splittedLine[3]);
            Version = splittedLine[4];
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

        #endregion
    }
}
