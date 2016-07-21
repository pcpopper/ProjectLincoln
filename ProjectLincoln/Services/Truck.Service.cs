using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class TruckService : ServiceAbstract {

        private List<Truck> trucks = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public TruckService () {
            trucks = new List<Truck>();
        }

        /// <summary>
        /// Static constructor and retrieval of all trucks
        /// </summary>
        /// <param name="units">Optional: A list of units to filter the results by</param>
        /// <returns>New instance of this class</returns>
        public static TruckService retrieveTrucks (List<int> units = null) {
            // Create a new instance of this class
            TruckService tService = new TruckService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Truck");

                // Set the where params of the units
                if (units != null && units.Count > 0) {
                    select.where("UnitId IN (??)", units);
                }

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    tService.trucks.Add(Truck.getFromDb(reader));
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
        /// Returns the list of trucks
        /// </summary>
        /// <returns>The list of trucks</returns>
        public List<Truck> getTrucks () {
            return trucks;
        }

        /// <summary>
        /// Gets the insert statements for the list
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the trucks
            foreach (Truck truck in trucks) {
                // Add the insert statement
                output.Add(truck.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
