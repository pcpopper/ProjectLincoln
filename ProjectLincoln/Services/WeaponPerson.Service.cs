using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class WeaponPersonService : ServiceAbstract {

        private List<WeaponPerson> joins = null;
        private List<Weapon> weapons = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public WeaponPersonService () {
            joins = new List<WeaponPerson>();
            weapons = new List<Weapon>();
        }

        /// <summary>
        /// Static constructor and retrieval of all weapons for the provided personnel
        /// </summary>
        /// <param name="personIds">The list of person ids</param>
        /// <returns>New instance of this class</returns>
        public static WeaponPersonService retrieveWeapons (List<int> personIds) {
            // Create a new instance of this class
            WeaponPersonService service = new WeaponPersonService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("WeaponPerson", "WP")
                    .join("Weapon", "W", "W.WeaponId = WP.WeaponId", JoinType.Inner)
                    .where("WP.PersonId IN (??)", personIds);

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    service.joins.Add(WeaponPerson.getFromDb(reader));
                    service.weapons.Add(Weapon.getFromDb(reader));
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
