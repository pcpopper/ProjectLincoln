using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class UnitService : ServiceAbstract {

        /// <summary>
        /// Gets the unit for the given unitId
        /// </summary>
        /// <param name="unitId">The id number of the requested unit</param>
        /// <returns>A unit class containing the requested unit</returns>
        public static Unit getUnit (int unitId) {
            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the output
                Unit output = null;

                // Create the querystring
                select = Select.create("Unit")
                        .where("UnitId = ?UnitId");

                // Open the connection
                db = MySQL.openConnection(select, new string[] {
                    "?UnitId",  unitId.ToString()});

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    output = Unit.getFromDb(reader);
                }

                // Close the connection and reader
                db.closeDB();

                // Return the output
                return output;
                #endregion
            } catch (Exception ex) {
                #region catch
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

                // Return nothing
                return null;
                #endregion
            }
        }
    }
}
