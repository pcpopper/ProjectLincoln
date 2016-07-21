using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class AttachmentTypeService : ServiceAbstract {

        private List<AttachmentType> types = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public AttachmentTypeService () {
            types = new List<AttachmentType>();
        }

        /// <summary>
        /// Static constructor and retrieval of all attachment types
        /// </summary>
        /// <returns>New instance of this class</returns>
        public static AttachmentTypeService retrieveTypes () {
            // Create a new instance of this class
            AttachmentTypeService atService = new AttachmentTypeService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("AttachmentType");

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    atService.types.Add(AttachmentType.getFromDb(reader));
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

            return atService;
        }

        /// <summary>
        /// Returns the list of attachment types
        /// </summary>
        /// <returns>The list of attachment types</returns>
        public List<AttachmentType> getServices () {
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
            foreach (AttachmentType type in types) {
                // Add the insert statement
                output.Add(type.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
