using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TWB_Parser
{
    class SQLServerDatabase
    {
        private string _connectionString;
        private SqlConnection _cnn;
        private CSVFileData _dataCSV;

        /// <summary>
        /// Constructor of MySQLDatabase
        /// </summary>
        /// <param name="aServerAddress">Network address of the server/computer where the database is located.</param>
        /// <param name="aDatabaseName">Name of the database instance.</param>
        /// <param name="aDatabaseUsername">Username of the account used to connect to the database.</param>
        /// <param name="aDatabasePassword">Password of the account used to connect to the database.</param>
        /// <param name="aDataCSV">Data from the TWB CSV file.</param>
        public SQLServerDatabase(string aServerAddress, string aDatabaseName, string aDatabaseUsername, string aDatabasePassword, CSVFileData aDataCSV)
        {
            _dataCSV = aDataCSV;
            //_connectionString = "Server = localhost\\SQLEXPRESS; Database = master; Trusted_Connection = True";
            _connectionString = String.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}",aServerAddress, aDatabaseName, aDatabaseUsername, aDatabasePassword);
            _cnn = new SqlConnection(_connectionString);
            Console.WriteLine("Connecting to the SQL Server database {0}:{1} as {2}...", aServerAddress, aDatabaseName, aDatabaseUsername);
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
        /// Insert the data from _dataCSV into the SQL Server database.
        /// </summary>
        private void InsertData()
        {
            SqlCommand command = _cnn.CreateCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sqlQuery;
            StringBuilder s = new StringBuilder();
            string groupId = "";
            SqlTransaction transaction;
            
            // Start a local transaction.
            transaction = _cnn.BeginTransaction("TBWTransaction");
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
                        s.Append("INSERT INTO groups(date, region, netplay, event, version, groupid) VALUES (");
                        s.Append("'").Append(line.Date).Append("',");
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
                    Console.WriteLine("ERROR while inserting the data. Attempting to rollback.");
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
