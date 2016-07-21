using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class BilletService: ServiceAbstract {

        private List<Billet> billets = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public BilletService () {
            billets = new List<Billet>();
        }

        /// <summary>
        /// Static constructor and retrieval of all services
        /// </summary>
        /// <returns>New instance of this class</returns>
        public static BilletService retrieveBillets () {
            // Create a new instance of this class
            BilletService bService = new BilletService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Billet");

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    bService.billets.Add(Billet.getFromDb(reader));
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
                            "An error has occurred: {1}\n{2}",
                            ex.Message,
                            ex.StackTrace));
                }
                #endregion
            }

            return bService;
        }

        /// <summary>
        /// Returns the list of billets
        /// </summary>
        /// <returns>The list of billets</returns>
        public List<Billet> getBillets () {
            return billets;
        }

        /// <summary>
        /// Gets the insert statements for the list
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the billets
            foreach (Billet billet in billets) {
                // Add the insert statement
                output.Add(billet.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
