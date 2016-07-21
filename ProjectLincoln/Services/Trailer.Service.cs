using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class TrailerService : ServiceAbstract {

        private List<Trailer> trailers = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public TrailerService () {
            trailers = new List<Trailer>();
        }

        /// <summary>
        /// Static constructor and retrieval of all trailers
        /// </summary>
        /// <param name="units">Optional: A list of units to filter the results by</param>
        /// <returns>New instance of this class</returns>
        public static TrailerService retrieveTrailers (List<int> units = null) {
            // Create a new instance of this class
            TrailerService tService = new TrailerService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Trailer");

                // Set the where params of the units
                if (units != null && units.Count > 0) {
                    select.where("UnitId IN (?)", units);
                }

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    tService.trailers.Add(Trailer.getFromDb(reader));
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

            return tService;
        }

        /// <summary>
        /// Returns the list of trailers
        /// </summary>
        /// <returns>The list of trailers</returns>
        public List<Trailer> getTrailers () {
            return trailers;
        }

        /// <summary>
        /// Gets the insert statements for the list
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the trailers
            foreach (Trailer trailer in trailers) {
                // Add the insert statement
                output.Add(trailer.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
