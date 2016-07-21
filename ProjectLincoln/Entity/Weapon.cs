using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Weapon {

        #region Properties
        public int? WeaponId { get; private set; } // The property for the table's id
        public string WeaponSerial { get; private set; } // The property for the foreign-key
        public int WeaponTypeId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weaponId">The weapon's id</param>
        /// <param name="weaponSerial">The weapon's serial</param>
        /// <param name="weaponTypeId">The weapon's type</param>
        public Weapon (int? weaponId, string weaponSerial, int weaponTypeId) {
            this.WeaponId = weaponId;
            this.WeaponSerial = weaponSerial;
            this.WeaponTypeId = weaponTypeId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Weapon getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new Weapon(
                reader.GetInt32("WeaponId"),
                reader.GetString("WeaponSerial"),
                reader.GetInt32("WeaponTypeId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Weapon{{WeaponId:{0}, WeaponSerial:{1}, WeaponTypeId:{2}}}",
                WeaponId,
                WeaponSerial,
                WeaponTypeId);
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
            fields.Add("WeaponId", WeaponId);
            fields.Add("WeaponSerial", WeaponSerial);
            fields.Add("WeaponTypeId", WeaponTypeId);

            // return the insert statement
            return Insert.create("Weapon", dbType, fields);
        }
    }
}
