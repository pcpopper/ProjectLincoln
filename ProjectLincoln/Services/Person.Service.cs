using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class PersonService: ServiceAbstract {

        private List<Person> persons = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public PersonService () {
            persons = new List<Person>();
        }

        /// <summary>
        /// Static constructor and retrieval of all persons
        /// </summary>
        /// <param name="units">Optional: A list of units to filter the results by</param>
        /// <returns>New instance of this class</returns>
        public static PersonService retrievePersons (List<int> units = null) {
            // Create a new instance of this class
            PersonService service = new PersonService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Person");

                // Set the where params of the units
                if (units != null && units.Count > 0) {
                    select.where("UnitId IN (??)", units);
                }

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    service.persons.Add(Person.getFromDb(reader));
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

            return service;
        }

        /// <summary>
        /// Returns the list of persons
        /// </summary>
        /// <returns>The list of persons</returns>
        public List<Person> getPersons () {
            return persons;
        }

        /// <summary>
        /// Gets the insert statements for the list
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the persons
            foreach (Person person in persons) {
                // Add the insert statement
                output.Add(person.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
