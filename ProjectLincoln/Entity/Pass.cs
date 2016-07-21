using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Pass {

        #region Properties
        public int? Id { get; private set; } // The property for the id
        public string Name { get; private set; } // The property for the name
        public string Abbr { get; private set; } // The property for the abbr
        public int? ServiceId { get; private set; } // The property for the serviceId
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The unit's id</param>
        /// <param name="name">The unit's name</param>
        /// <param name="abbr">The unit's abbreviation</param>
        /// <param name="serviceId">Optional: The service that the unit belongs to</param>
        public Pass (int? id, string name, string abbr, int? serviceId) {
            this.Id = id;
            this.Name = name;
            this.Abbr = abbr;
            this.ServiceId = serviceId;
        }

        /// <summary>
        /// Creates and returns a new unit object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Unit getUnitFromDb (MySqlDataReader reader) {
            // Error check the serviceId
            int? serviceId = null;
            if (!Convert.IsDBNull(reader["ServiceId"])) {
                serviceId = reader.GetInt32("ServiceId");
            }

            // Return a new Unit object with the provided values
            return new Unit(
                reader.GetInt32("UnitId"),
                reader.GetString("Name"),
                reader.GetString("Abbreviation"),
                serviceId);
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Unit{{id:{0}, name:{1}, abbr:{2}, serviceId:{3}}}",
                Id,
                Name,
                Abbr,
                ServiceId);
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
            fields.Add("UnitId", Id);
            fields.Add("Name", Name);
            fields.Add("Abbreviation", Abbr);
            fields.Add("ServiceId", ServiceId);

            // return the insert statement
            return Insert.create("Unit", dbType, fields);
        }
    }
}
