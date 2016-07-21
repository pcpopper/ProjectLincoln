using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Service {

        #region Properties
        public int? ServiceId { get; private set; } // The property for the table's id
        public string Name { get; private set; } // The property for the field
        public string Abbreviation { get; private set; } // The property for the field
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceId">Optional: The service that the unit belongs to</param>
        /// <param name="name">The unit's name</param>
        /// <param name="abbreviation">The unit's abbreviation</param>
        public Service (int? serviceId, string name, string abbreviation) {
            this.ServiceId = serviceId;
            this.Name = name;
            this.Abbreviation = abbreviation;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Service getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new Service(
                reader.GetInt32("ServiceId"),
                reader.GetString("Name"),
                reader.GetString("Abbreviation"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Services{{ServiceId:{0}, Name:{1}, Abbreviation:{2}}}",
                ServiceId,
                Name,
                Abbreviation);
        }

        /// <summary>
        /// Method that returns the insert statement for this class
        /// </summary>
        /// <param name="dbType">The type of db that this will be inserted into</param>
        /// <returns>The insert statement</returns>
        public string ToInsert (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the table's fields
            Dictionary<string, Object> fields = new Dictionary<string, Object> ();

            // Add the table's fields
            fields.Add("ServiceId", ServiceId);
            fields.Add("Name", Name);
            fields.Add("Abbreviation", Abbreviation);

            // return the insert statement
            return Insert.create("Services", dbType, fields);
        }
    }
}
