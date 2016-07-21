using System;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Properties;
using System.Windows.Forms;

namespace ProjectLincoln.Helpers.DB {
    public class MySQL : DatabaseAbstract {

        public bool hasChecked = false, updatesAvailable = false;
        public MySqlConnection conn = null; // The database connection
        public MySqlCommand comm = null; // The database command object
        public MySqlDataReader reader = null; // The database reader

        /// <summary>
        /// Constructor
        /// </summary>
        public MySQL () {
            // Initialize all of the database objects
            buildDbObjects();
        }

        /// <summary>
        /// Method that creates, instantiates, and returns an instance of this class
        /// </summary>
        /// <param name="query">The Select object</param>
        /// <param name="parameters">An array of parameter variable and value strings</param>
        /// <returns></returns>
        public static MySQL openConnection (Select query, params string[] parameters) {
            // Create a new instance of this class
            MySQL newSql = new MySQL();

            try {
                // Open the connection and connect
                newSql.conn = new MySqlConnection(newSql.connectionString);
                newSql.conn.Open();

                // Prepaire the command
                newSql.comm = new MySqlCommand(query.ToString(), newSql.conn);
                newSql.comm.Prepare();

                // Assign the parameter(s) if provided
                if (parameters.Length > 0) {
                    for (int i = 0; i < parameters.Length - 1; i += 2) {
                        newSql.comm.Parameters.AddWithValue(parameters[i], parameters[i + 1]);
                    }
                }

                // Create a reader
                newSql.reader = newSql.comm.ExecuteReader();

                // Return this object
                return newSql;
            } catch (MySql.Data.MySqlClient.MySqlException ex) {
                // Show the error message
                if (Settings.Default.DevMode) {
                    Debug.WriteLine(
                        String.Format(
                            "Error {0} has occurred: {1}\n{2}",
                            ex.Number,
                            ex.Message,
                            ex.StackTrace));
                }

                // Close the connection
                newSql.closeDB();

                return null;
            }
        }

        /// <summary>
        /// Close the connection and reader if they are not already
        /// </summary>
        public void closeDB () {
            // Try closing
            if (reader != null && !reader.IsClosed) {
                reader.Close();
            }
            if (conn != null && conn.State != ConnectionState.Closed) {
                conn.Close();
            }
        }

        #region Public Methods
        internal bool checkForUpdates () {
            throw new NotImplementedException();
        }

        #endregion
    }
}
