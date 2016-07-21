using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class WeaponPerson {

        #region Properties
        public int? WeaponPersonId { get; private set; } // The property for the table's id
        public int WeaponId { get; private set; } // The property for the foreign-key
        public int PersonId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weaponPersonId">The weaponPerson's id</param>
        /// <param name="weaponId">The weapon's id</param>
        /// <param name="personId">The person's id</param>
        public WeaponPerson (int? weaponPersonId, int weaponId, int personId) {
            this.WeaponPersonId = weaponPersonId;
            this.WeaponId = weaponId;
            this.PersonId = personId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static WeaponPerson getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new WeaponPerson(
                reader.GetInt32("WeaponPersonId"),
                reader.GetInt32("WeaponId"),
                reader.GetInt32("PersonId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "WeaponPerson{{WeaponPersonId:{0}, WeaponId:{1}, PersonId:{2}}}",
                WeaponPersonId,
                WeaponId,
                PersonId);
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
            fields.Add("WeaponPersonId", WeaponPersonId);
            fields.Add("WeaponId", WeaponId);
            fields.Add("PersonId", PersonId);

            // return the insert statement
            return Insert.create("WeaponPerson", dbType, fields);
        }
    }
}
