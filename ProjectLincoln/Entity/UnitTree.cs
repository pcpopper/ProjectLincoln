using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    class UnitTree {

        #region Properties
        public int UnitTreeId { get; private set; } // The property for the table's id
        public int UnitId { get; private set; } // The property for the foreign-key
        public int ParentId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitTreeId">The unit's tree id</param>
        /// <param name="unitId">The unit's id</param>
        /// <param name="parentId">Optional: The parent unit's id that the unit falls under</param>
        public UnitTree (int unitTreeId, int unitId, int parentId) {
            this.UnitTreeId = unitTreeId;
            this.UnitId = unitId;
            this.ParentId = parentId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static UnitTree getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new UnitTree(
                reader.GetInt32("UnitTreeId"),
                reader.GetInt32("UnitId"),
                reader.GetInt32("ParentId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Unit{{UnitTreeId:{0}, UnitId:{1}, ParentId:{2}}}",
                UnitTreeId,
                UnitId,
                ParentId);
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
            fields.Add("UnitTreeId", UnitTreeId);
            fields.Add("UnitId", UnitId);
            fields.Add("ParentId", ParentId);

            // return the insert statement
            return Insert.create("UnitTree", dbType, fields);
        }
    }
}
