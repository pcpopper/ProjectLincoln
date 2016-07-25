using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class WeaponTruckService : ServiceAbstract {

        private List<WeaponTruck> joins = null;
        private List<Weapon> weapons = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public WeaponTruckService () {
            joins = new List<WeaponTruck>();
            weapons = new List<Weapon>();
        }

        /// <summary>
        /// Static constructor and retrieval of all weapons for the provided trucks
        /// </summary>
        /// <param name="truckIds">The list of truck ids</param>
        /// <returns>New instance of this class</returns>
        public static WeaponTruckService retrieveWeapons (List<int> truckIds) {
            // Create a new instance of this class
            WeaponTruckService wtService = new WeaponTruckService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("WeaponTruck", "WT")
                    .join("Weapon", "W", "W.WeaponId = WT.WeaponId", JoinType.Inner)
                    .where("WT.TruckId IN (??)", truckIds);

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    wtService.joins.Add(WeaponTruck.getFromDb(reader));
                    wtService.weapons.Add(Weapon.getFromDb(reader));
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

            return wtService;
        }

        /// <summary>
        /// Returns the list of weapons
        /// </summary>
        /// <returns>The list of weapons</returns>
        public List<Weapon> getWeapons () {
            return weapons;
        }

        /// <summary>
        /// Gets the insert statements for the list
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the two lists
            for (int i = 0; i < Math.Min(joins.Count, weapons.Count); i++) {
                // Add the insert statements
                output.Add(weapons[i].ToInsert(dbType));
                output.Add(joins[i].ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
