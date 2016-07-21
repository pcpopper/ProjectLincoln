using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Helpers.Encryption;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class Manifest : ServiceAbstract {
        /// <summary>
        /// Method that checks if the login is correct and stores the setting if not present
        /// </summary>
        /// <param name="username">The username that was supplied</param>
        /// <param name="password">The password that was supplied</param>
        /// <returns></returns>
        public static bool checkCredentials (string username, string password) {
            // Prepare output
            bool loggedIn = false;

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Users", "U")
                    .join("Pass", "U.UserId = P.UserId", JoinType.Inner, "P")
                    .where("Username = ?username");

                // Open the connection
                db = MySQL.openConnection(select, new string[] {
                    "?username", username});

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    // Check the generated hash against the stored hash
                    if (OneWayEncryption.checkPassword(password, reader.GetString("Pass"))) {
                        // Store the username and hash into the settings
                        Settings.Default.Username = username;
                        Settings.Default.Password = password;

                        // Store the unitId if there is one in the db
                        if (!Convert.IsDBNull(reader["UnitId"])) {
                            Settings.Default.UnitId = reader.GetInt32("UnitId");
                        }

                        // Set the logged in flag
                        loggedIn = true;
                    }
                }

                // Close the connection and reader
                db.closeDB();

                // Return the loggedIn value
                return loggedIn;
                #endregion
            } catch (Exception ex) {
                #region Catch
                // Try to close the connection and reader
                if (db != null) {
                    db.closeDB();
                }

                // Show the error message
                if (Settings.Default.IsDev) {
                    Debug.WriteLine(
                        String.Format(
                            "An error has occurred: {1}\n{2}",
                            ex.Message,
                            ex.StackTrace));
                }

                // Return false as there was an error
                return false;
                #endregion
            }
        }
    }
}
