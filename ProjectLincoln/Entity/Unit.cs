using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Unit {

        #region Properties
        public int? UnitId { get; private set; } // The property for the table's id
        public string Name { get; private set; } // The property for the field
        public string Abbreviation { get; private set; } // The property for the field
        public int? UnitLevelId { get; private set; } // The property for the foreign-key
        public int? ServiceId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitId">The unit's id</param>
        /// <param name="name">The unit's name</param>
        /// <param name="abbreviation">The unit's abbreviation</param>
        /// <param name="unitLevelId">Optional: The unit's level</param>
        /// <param name="serviceId">Optional: The service that the unit belongs to</param>
        public Unit (int? unitId, string name, string abbreviation, int? unitLevelId, int? serviceId) {
            this.UnitId = unitId;
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.UnitLevelId = unitLevelId;
            this.ServiceId = serviceId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Unit getFromDb (MySqlDataReader reader) {
            // Error check the unitLevelId
            int? unitLevelId = null;
            if (!Convert.IsDBNull(reader["UnitLevelId"])) {
                unitLevelId = reader.GetInt32("UnitLevelId");
            }
            // Error check the serviceId
            int? serviceId = null;
            if (!Convert.IsDBNull(reader["ServiceId"])) {
                serviceId = reader.GetInt32("ServiceId");
            }

            // Return a new object with the provided values
            return new Unit(
                reader.GetInt32("UnitId"),
                reader.GetString("Name"),
                reader.GetString("Abbreviation"),
                unitLevelId,
                serviceId);
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Unit{{UnitId:{0}, Name:{1}, Abbreviation:{2}, UnitLevelId: {3}, ServiceId:{4}}}",
                UnitId,
                Name,
                Abbreviation,
                UnitLevelId,
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
            fields.Add("UnitId", UnitId);
            fields.Add("Name", Name);
            fields.Add("Abbreviation", Abbreviation);
            fields.Add("UnitLevelId", UnitLevelId);
            fields.Add("ServiceId", ServiceId);

            // return the insert statement
            return Insert.create("Unit", dbType, fields);
        }
    }
}
