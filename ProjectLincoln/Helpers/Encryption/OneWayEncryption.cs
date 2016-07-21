using ProjectLincoln.Properties;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ProjectLincoln.Helpers.Encryption {
    /// <summary>
    /// Helper class for BCrypt encryption
    /// </summary>
    class OneWayEncryption {

        /// <summary>
        /// Static method that generates a salt from the encryption cost in the settings
        /// </summary>
        /// <returns>The 29 character salt string</returns>
        public static string generateSalt () {
            try {
                return BCrypt.Net.BCrypt.GenerateSalt(Settings.Default.PasswordBCryptCost);
            } catch (BCrypt.Net.SaltParseException e) {
                if (Settings.Default.DevMode) {
                    MessageBox.Show(e.Message + "\n\n" + e.StackTrace);
                }

                return "";
            }
        }

        /// <summary>
        /// Method that checks the provided password against the stored hash for the user
        /// </summary>
        /// <param name="password">The entered password to check</param>
        /// <param name="hashedPassword">The hashed password for the user</param>
        /// <returns>The boolean status of if the password and hash match</returns>
        public static bool checkPassword (string password, string hashedPassword) {
            try {
                return BCrypt.Net.BCrypt.Verify(
                    password + Settings.Default.DBPass,
                    hashedPassword);
            } catch (BCrypt.Net.SaltParseException e) {
                if (Settings.Default.DevMode) {
                    MessageBox.Show(e.Message + "\n\n" + e.StackTrace);
                }

                return false;
            }
        }

        /// <summary>
        /// Generates a hash from the supplied parameters
        /// </summary>
        /// <param name="password">The requested password</param>
        /// <param name="salt">Optional: A 29 chacter salt</param>
        /// <returns>A dictionary containing the 29 character salt string and the hash</returns>
        public static Dictionary<string, string> generateHash (string password, string salt = null) {
            try {
                // Check if the provided salt is null or if it does not adhere to the rules
                if (salt == null || !checkSalt(salt)) {
                    // Generate a new salt
                    salt = generateSalt();
                }

                // Initialize the output
                Dictionary<string, string> output = new Dictionary<string, string>();

                // Add the salt and new hash
                output.Add("salt", salt);
                output.Add("hash", BCrypt.Net.BCrypt.HashPassword(password + Settings.Default.DBPass, salt));

                // Return the salt and hash
                return output;
            } catch (BCrypt.Net.SaltParseException e) {
                if (Settings.Default.DevMode) {
                    MessageBox.Show(e.Message + "\n\n" + e.StackTrace);
                }

                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Checks if the salt is long enough and has the right version and artifacts
        /// </summary>
        /// <param name="salt">The salt to verify</param>
        /// <returns>The validity of the salt</returns>
        private static bool checkSalt (string salt) {
            // Create the regex
            Regex regex = new Regex(@"^\$2a\$[0-9]{2}\$[a-z0-9/]{22}$");

            // Return the matching status
            return (regex.IsMatch(salt));
        }
    }
}
