using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class UnitLevelService : ServiceAbstract {

        private List<UnitLevel> unitLevels = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitLevelService () {
            unitLevels = new List<UnitLevel>();
        }

        /// <summary>
        /// Static constructor and retrieval of all unitLevels
        /// </summary>
        /// <returns>New instance of this class</returns>
        public static UnitLevelService retrieveUnitLevels () {
            // Create a new instance of this class
            UnitLevelService service = new UnitLevelService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("UnitLevel");

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    service.unitLevels.Add(UnitLevel.getFromDb(reader));
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

            return service;
        }

        /// <summary>
        /// Returns the list of UnitLevels
        /// </summary>
        /// <returns>The list of UnitLevels</returns>
        public List<UnitLevel> getUnitLevels () {
            return unitLevels;
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
            foreach (UnitLevel unitLevel in unitLevels) {
                // Add the insert statement
                output.Add(unitLevel.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
