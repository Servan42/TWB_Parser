using System;
using System.Collections.Generic;
using System.Configuration;
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
                "| Version                           1.3.0 |\n" +
                "| Release Date                 2021-03-27 |\n" +
                "| Author & Maintainer           @Servan42 |\n" +
                "|-----------------------------------------|\n");


            // Maybe un-comment the following line if you have date problems.
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            try
            {
                // Read the config.
                Console.WriteLine("Loading the configuration from TWB_Parser.exe.config...");
                if (!File.Exists("TWB_Parser.exe.config")) throw new FileNotFoundException("ERROR: Config file not found.");
                if(ConfigurationManager.AppSettings.Get("DatabaseType") != "MySQL" && ConfigurationManager.AppSettings.Get("DatabaseType") != "SQLServer")
                {
                    throw new FormatException("ERROR: Invalid database type in TWB_Parser.exe.config. The program only supports \"MySQL\" and \"SQLServer\".");
                }
                Console.WriteLine("DONE\n");
                
                // Parse the CSV file
                Console.WriteLine("Loading the data from \"{0}\"...", ConfigurationManager.AppSettings.Get("CSVMatchsFilename"));
                CSVFileData dataCSV = new CSVFileData(ConfigurationManager.AppSettings.Get("CSVMatchsFilename"));
                Console.WriteLine("DONE - {0} matches found.\n", dataCSV.Lines.Count);
                
                // Connect to the database, insert the information, and disconnect.
                if(ConfigurationManager.AppSettings.Get("DatabaseType") == "SQLServer")
                {
                    SQLServerDatabase sqlServerDB = new SQLServerDatabase(ConfigurationManager.AppSettings.Get("ServerAddress"),
                        ConfigurationManager.AppSettings.Get("DatabaseName"),
                        ConfigurationManager.AppSettings.Get("DatabaseUsername"),
                        ConfigurationManager.AppSettings.Get("DatabasePassword"), 
                        dataCSV);
                    Console.WriteLine("\nSuccessful.");
                }
                else if (ConfigurationManager.AppSettings.Get("DatabaseType") == "MySQL")
                {
                    MySQLDatabase mySqlDB = new MySQLDatabase(ConfigurationManager.AppSettings.Get("ServerAddress"),
                        ConfigurationManager.AppSettings.Get("PortMySQL"),
                        ConfigurationManager.AppSettings.Get("DatabaseName"),
                        ConfigurationManager.AppSettings.Get("DatabaseUsername"),
                        ConfigurationManager.AppSettings.Get("DatabasePassword"),
                        dataCSV);
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
