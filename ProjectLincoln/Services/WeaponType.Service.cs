using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class WeaponTypeService : ServiceAbstract {

        private List<WeaponType> types = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public WeaponTypeService () {
            types = new List<WeaponType>();
        }

        /// <summary>
        /// Static constructor and retrieval of all weapon types
        /// </summary>
        /// <returns>New instance of this class</returns>
        public static WeaponTypeService retrieveTypes () {
            // Create a new instance of this class
            WeaponTypeService wService = new WeaponTypeService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("WeaponType");

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    wService.types.Add(WeaponType.getFromDb(reader));
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

            return wService;
        }

        /// <summary>
        /// Returns the list of weapon types
        /// </summary>
        /// <returns>The list of weapon types</returns>
        public List<WeaponType> getServices () {
            return types;
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
            foreach (WeaponType type in types) {
                // Add the insert statement
                output.Add(type.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
