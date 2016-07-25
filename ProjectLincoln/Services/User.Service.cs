using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Helpers.Encryption;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class UserService: ServiceAbstract {

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
                    .join("Pass", "P", "U.UserId = P.UserId", JoinType.Inner)
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

                        // Get the rights
                        if (!Convert.IsDBNull(reader["IsDevAdmin"])) {
                            Settings.Default.IsDevAdmin = reader.GetBoolean("IsDevAdmin");
                        }
                        if (!Convert.IsDBNull(reader["IsUnitAdmin"])) {
                            Settings.Default.IsUnitAdmin = reader.GetBoolean("IsUnitAdmin");
                        }
                        if (!Convert.IsDBNull(reader["IsUserAdmin"])) {
                            Settings.Default.IsUserAdmin = reader.GetBoolean("IsUserAdmin");
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
                if (Settings.Default.DevMode) {
                    Debug.WriteLine(
                        String.Format(
                            "An error has occurred: {0}\n{1}",
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
