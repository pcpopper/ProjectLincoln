using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class TamcnService : ServiceAbstract {

        private List<Tamcn> tamcns = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public TamcnService () {
            tamcns = new List<Tamcn>();
        }

        /// <summary>
        /// Static constructor and retrieval of a tamcn
        /// </summary>
        /// <param name="tamcn">The id of the record to retrieve</param>
        /// <returns>New instance of this class</returns>
        public static Tamcn retrieveTamcn (int tamcnId) {
            //// Create a new instance of this class
            //TamcnService tamcn = new TamcnService();

            //// Prepare the db
            //MySQL db = null;

            //try {
            //    #region Try
            //    // Create the querystring
            //    select = Select.create("Tamcn");

            //    // Open the connection
            //    db = MySQL.openConnection(select);

            //    // Read the rows
            //    MySqlDataReader reader = db.reader;
            //    while (db.reader.Read()) {
            //        tamcn.tamcns.Add(Tamcn.getFromDb(reader));
            //    }

            //    // Close the connection and reader
            //    db.closeDB();
            //    #endregion
            //} catch (Exception ex) when (ex is FormatException) {
            //    #region Catch
            //    // Try to close the connection and reader
            //    if (db != null) {
            //        db.closeDB();
            //    }

            //    // Show the error message
            //    if (Settings.Default.DevMode) {
            //        Debug.WriteLine(
            //            String.Format(
            //                "An error has occurred: {0}\n{1}",
            //                ex.Message,
            //                ex.StackTrace));
            //    }
            //    #endregion
            //}
            Tamcn tamcn = new Tamcn(tamcnId, "D0023", "MRAP Cougar, 6X^", "MRAP CAT II", 9, true, false, false, false, "mrap 6x6.png");

            return tamcn;
        }
        
        /// <summary>
        /// Static constructor and retrieval of all tamcns
        /// </summary>
        /// <returns>New instance of this class</returns>
        public static TamcnService retrieveTamcns () {
            // Create a new instance of this class
            TamcnService tamcn = new TamcnService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Tamcn");

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    tamcn.tamcns.Add(Tamcn.getFromDb(reader));
                }

                // Close the connection and reader
                db.closeDB();
                #endregion
            } catch (Exception ex) when (ex is FormatException) {
                #region Catch
                // Try to close the connection and reader
                if (db != null) {
                    db.closeDB();
                }

                // Show the error message
                if (Settings.Default.DevMode) {
                    Debug.WriteLine(
                        String.Format(
                            "An error has occurred: {0}\n{1}",
                            ex.Message,
                            ex.StackTrace));
                }
                #endregion
            }

            return tamcn;
        }

        /// <summary>
        /// Returns the list of tamcns
        /// </summary>
        /// <returns>The list of tamcns</returns>
        public List<Tamcn> getServices () {
            return tamcns;
        }

        /// <summary>
        /// Gets the insert statements for the list
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the ranks
            foreach (Tamcn tamcn in tamcns) {
                // Add the insert statement
                output.Add(tamcn.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
