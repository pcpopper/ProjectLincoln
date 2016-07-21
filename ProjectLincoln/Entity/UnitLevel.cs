using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    class UnitLevel {

        #region Properties
        public int? UnitLevelId { get; private set; } // The property for the table's id
        public int Level { get; private set; } // The property for the field
        public string Name { get; private set; } // The property for the field
        public string Short { get; private set; } // The property for the field
        public int ServiceId { get; private set; } // The property for the field
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitLevelId">Optional: The unitlevel's id</param>
        /// <param name="level">The unitlevel's code</param>
        /// <param name="name">The unitlevel's name</param>
        /// <param name="shortDesc">The unitlevel's short description</param>
        /// <param name="serviceId">The unitlevel's service id</param>
        public UnitLevel (int? unitLevelId, int level, string name, string shortDesc, int serviceId) {
            this.UnitLevelId = unitLevelId;
            this.Level = level;
            this.Name = name;
            this.Short = shortDesc;
            this.ServiceId = serviceId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static UnitLevel getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new UnitLevel(
                reader.GetInt32("UnitLevelId"),
                reader.GetInt32("Level"),
                reader.GetString("Name"),
                reader.GetString("Short"),
                reader.GetInt32("ServiceId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "UnitLevel{{BilletId:{0}, Code:{1}, Name: {2}, Short: {3}, ServiceId: {4}}}",
                UnitLevelId,
                Level,
                Name,
                Short,
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
            fields.Add("UnitLevelId", UnitLevelId);
            fields.Add("Level", Level);
            fields.Add("Name", Name);
            fields.Add("Short", Short);
            fields.Add("ServiceId", ServiceId);

            // return the insert statement
            return Insert.create("UnitLevel", dbType, fields);
        }
    }
}
