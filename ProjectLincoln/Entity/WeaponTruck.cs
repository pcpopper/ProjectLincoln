using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class WeaponTruck {

        #region Properties
        public int? WeaponTruckId { get; private set; } // The property for the  table's id
        public int WeaponId { get; private set; } // The property for the foreign-key
        public int TruckId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weaponTruckId">The weaponTruck's id</param>
        /// <param name="weaponId">The weapon's id</param>
        /// <param name="truckId">The truck's id</param>
        public WeaponTruck (int? weaponTruckId, int weaponId, int truckId) {
            this.WeaponTruckId = weaponTruckId;
            this.WeaponId = weaponId;
            this.TruckId = truckId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static WeaponTruck getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new WeaponTruck(
                reader.GetInt32("WeaponTruckId"),
                reader.GetInt32("WeaponId"),
                reader.GetInt32("TruckId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "WeaponTruck{{WeaponTruckId:{0}, WeaponId:{1}, TruckId:{2}}}",
                WeaponTruckId,
                WeaponId,
                TruckId);
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
            fields.Add("WeaponTruckId", WeaponTruckId);
            fields.Add("WeaponId", WeaponId);
            fields.Add("TruckId", TruckId);

            // return the insert statement
            return Insert.create("WeaponTruck", dbType, fields);
        }
    }
}
