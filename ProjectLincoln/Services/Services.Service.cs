using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class ServicesService : ServiceAbstract {

        private List<Service> services = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ServicesService () {
            services = new List<Service>();
        }

        /// <summary>
        /// Static constructor and retrieval of all services
        /// </summary>
        /// <returns>New instance of this class</returns>
        public static ServicesService retrieveServices () {
            // Create a new instance of this class
            ServicesService sService = new ServicesService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Services");

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    sService.services.Add(Service.getFromDb(reader));
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

            return sService;
        }

        /// <summary>
        /// Returns the list of services
        /// </summary>
        /// <returns>The list of services</returns>
        public List<Service> getServices () {
            return services;
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
            foreach (Service service in services) {
                // Add the insert statement
                output.Add(service.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
