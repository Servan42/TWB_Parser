using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TWB_Parser
{
    class MySQLDatabase
    {
        private string _connectionString;
        private MySqlConnection _cnn;
        private CSVFileData _dataCSV;
        private string _databaseName;

        /// <summary>
        /// Constructor of MySQLDatabase
        /// </summary>
        /// <param name="aServerAddress">Network address of the server/computer where the database is located.</param>
        /// <param name="aPort">Port used to connect to the MySQL database. 3306 by default.</param>
        /// <param name="aDatabaseName">Name of the database instance.</param>
        /// <param name="aDatabaseUsername">Username of the account used to connect to the database.</param>
        /// <param name="aDatabasePassword">Password of the account used to connect to the database.</param>
        /// <param name="aDataCSV">Data from the TWB CSV file.</param>
        public MySQLDatabase(string aServerAddress, string aPort, string aDatabaseName, string aDatabaseUsername, string aDatabasePassword, CSVFileData aDataCSV)
        {
            if (string.IsNullOrWhiteSpace(aPort)) aPort = "3306";
            _dataCSV = aDataCSV;
            _databaseName = aDatabaseName;
            //_connectionString = "Server = localhost\\SQLEXPRESS; Database = master; Trusted_Connection = True";
            _connectionString = String.Format(@"Server={0};Port={1};Database={2};UID={3};Password={4}", aServerAddress, aPort ,aDatabaseName, aDatabaseUsername, aDatabasePassword);
            _cnn = new MySqlConnection(_connectionString);
            Console.WriteLine("Connecting to the MySQL database {0}:{1} as {2}...", aServerAddress, aDatabaseName, aDatabaseUsername);
            _cnn.Open();
            Console.WriteLine("DONE\n");
            Console.WriteLine("Inserting the data into the database...");
            InsertData();
            Console.WriteLine("DONE\n");
            Console.WriteLine("Closing the connection...");
            _cnn.Close();
            Console.WriteLine("DONE\n");
        }
        
        /// <summary>
        /// Insert the data from _dataCSV into the MySQL database.
        /// </summary>
        private void InsertData()
        {
            MySqlCommand command = _cnn.CreateCommand();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            string sqlQuery;
            StringBuilder s = new StringBuilder();
            string groupId = "";
            MySqlTransaction transaction;

            // Start a local transaction.
            transaction = _cnn.BeginTransaction();
            command.Transaction = transaction;
            command.Connection = _cnn;

            try
            {
                // Inserting the matches into the database.
                foreach (CSVFileLine line in _dataCSV.Lines)
                {
                    s.Clear();
                    s.Append("INSERT INTO matches(p1name, p1char1, p1char2, p1char3, p2name, p2char1, p2char2, p2char3, link, precedence, groupid) VALUES (");
                    s.Append("'").Append(line.P1Name).Append("',");
                    s.Append("'").Append(line.P1Char1).Append("',");
                    s.Append("'").Append(line.P1Char2).Append("',");
                    s.Append("'").Append(line.P1Char3).Append("',");
                    s.Append("'").Append(line.P2Name).Append("',");
                    s.Append("'").Append(line.P2Char1).Append("',");
                    s.Append("'").Append(line.P2Char2).Append("',");
                    s.Append("'").Append(line.P2Char3).Append("',");
                    s.Append("'").Append(line.URL).Append("',");
                    s.Append("'").Append(line.Precedence).Append("',");
                    s.Append("'").Append(line.GroupId).Append("'");
                    s.Append(");");
                    sqlQuery = s.ToString();
                    //command = new SqlCommand(sqlQuery, _cnn);
                    command.CommandText = sqlQuery;
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    //command.Dispose();

                    // If the group is not in the base yet, we insert it.
                    if (line.GroupId != groupId)
                    {
                        groupId = line.GroupId;

                        s.Clear();
                        s.Append("INSERT INTO ").Append(_databaseName).Append(".groups(date, region, netplay, event, version, groupid) VALUES (");
                        s.Append("'").Append(line.Date.ToString("yyyy-MM-dd H:mm:ss")).Append("',");
                        s.Append("'").Append(line.Region).Append("',");
                        s.Append("'").Append(line.Netplay).Append("',");
                        s.Append("'").Append(line.Event).Append("',");
                        s.Append("'").Append(line.Version).Append("',");
                        s.Append("'").Append(line.GroupId).Append("'");
                        s.Append(");");
                        sqlQuery = s.ToString();
                        //command = new SqlCommand(sqlQuery, _cnn);
                        command.CommandText = sqlQuery;
                        adapter.InsertCommand = command;
                        adapter.InsertCommand.ExecuteNonQuery();
                        //command.Dispose();
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Attempt to roll back the transaction.
                try
                {
                    Console.WriteLine("Error while inserting the data. Attempting to rollback.");
                    transaction.Rollback();
                    Console.WriteLine("Rollback successful.");
                }
                catch (Exception ex2)
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }

                command.Dispose();
                _cnn.Close();
                throw ex;
            }

            command.Dispose();
        } 
    }
}
