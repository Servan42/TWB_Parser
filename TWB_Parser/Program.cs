using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace TWB_Parser
{
    class Program
    {
        /// <summary>
        /// Entry point of the program.
        /// </summary>
        /// <param name="args">Unsued. Everything goes through the config.xml file.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("|-----------------------------------------|\n" +
                "| CSV Match Parser for tunawithbeacon.com |\n" +
                "|-----------------------------------------|\n" +
                "| Version                           1.1.1 |\n" +
                "| Release Date                 2019-08-18 |\n" +
                "| Author & Maintainer           @Servan42 |\n" +
                "|-----------------------------------------|\n");


            // Maybe un-comment the following line if you have date problems.
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            try
            {
                // Read the config.
                Console.WriteLine("Loading the configuration from config.xml...");
                List<String> config = Tools.XmlReader(Directory.GetCurrentDirectory() + "\\config.xml");
                if(config[1] != "MySQL" && config[1] != "SQLServer")
                {
                    throw new FormatException("ERROR: Invalid database type in config.xml. The program only supports \"MySQL\" and \"SQLServer\".");
                }
                Console.WriteLine("DONE\n");
                
                // Parse the CSV file
                Console.WriteLine("Loading the data from \"{0}\"...", config[0]);
                CSVFileData dataCSV = new CSVFileData(config[0]);
                Console.WriteLine("DONE - {0} matches found.\n", dataCSV.Lines.Count);
                
                // Connect to the database, insert the information, and disconnect.
                if(config[1] == "SQLServer")
                {
                    SQLServerDatabase sqlServerDB = new SQLServerDatabase(config[2], config[4], config[5], config[6], dataCSV);
                    Console.WriteLine("\nSuccessful.");
                }
                else if (config[1] == "MySQL")
                {
                    MySQLDatabase mySqlDB = new MySQLDatabase(config[2], config[3], config[4], config[5], config[6], dataCSV);
                    Console.WriteLine("\nSuccessful.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Stacktrace for debugging.
                // Console.WriteLine("\n" + ex.StackTrace);
                Console.WriteLine("\nAborting.");
            }
            Console.WriteLine("\nPress any key to close.");
            Console.ReadKey();
        }
    }
}
