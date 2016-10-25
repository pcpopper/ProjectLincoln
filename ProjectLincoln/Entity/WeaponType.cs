using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class WeaponType {

        #region Properties
        public int? WeaponTypeId { get; private set; } // The property for the table's id
        public int? TamcnId { get; private set; } // The property for the foreign-key
        public bool IsPersonnel { get; private set; } // The property for the field
        public bool IsTruck { get; private set; } // The property for the field
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weaponTypeId">The weapon type's id</param>
        /// <param name="tamcnId">Optional: The weapon type's tamcn</param>
        /// <param name="IsPersonnel">Is this for a person?</param>
        /// <param name="isTruck">Is this for a truck?</param>
        public WeaponType (int? weaponTypeId, int? tamcnId, bool isPersonnel, bool isTruck) {
            this.WeaponTypeId = weaponTypeId;
            this.TamcnId = tamcnId;
            this.IsPersonnel = isPersonnel;
            this.IsTruck = isTruck;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static WeaponType getFromDb (MySqlDataReader reader) {

            // Return a new object with the provided values
            return new WeaponType(
                reader.GetInt32("WeaponTypeId"),
                reader.GetInt32("TamcnId"),
                reader.GetBoolean("IsPersonnel"),
                reader.GetBoolean("IsTruck"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "WeaponType{{WeaponTypeId:{0}, TamcnId:{1}, IsPersonnel:{2}, IsTruck:{3}}}",
                WeaponTypeId,
                TamcnId,
                IsPersonnel,
                IsTruck);
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
            fields.Add("WeaponTypeId", WeaponTypeId);
            fields.Add("TamcnId", TamcnId);
            fields.Add("IsPersonnel", IsPersonnel);
            fields.Add("IsTruck", IsTruck);

            // return the insert statement
            return Insert.create("WeaponType", dbType, fields);
        }
    }
}
