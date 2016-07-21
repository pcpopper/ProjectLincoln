using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class AttachmentWeapon {

        #region Properties
        public int? AttachmentWeaponId { get; private set; } // The property for the  table's id
        public int AttachmentId { get; private set; } // The property for the foreign-key
        public int WeaponId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attachmentWeaponId">The attachmentWeapon's id</param>
        /// <param name="attachmentId">The attachmentWeapon's attachment id</param>
        /// <param name="weaponId">The attachmentWeapon's weapon id</param>
        public AttachmentWeapon (int? attachmentWeaponId, int attachmentId, int weaponId) {
            this.AttachmentWeaponId = attachmentWeaponId;
            this.AttachmentId = attachmentId;
            this.WeaponId = weaponId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static AttachmentWeapon getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new AttachmentWeapon(
                reader.GetInt32("AttachmentWeaponId"),
                reader.GetInt32("AttachmentId"),
                reader.GetInt32("WeaponId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "AttachmentWeapon{{AttachmentWeaponId:{0}, AttachmentId:{1}, WeaponId:{2}}}",
                AttachmentWeaponId,
                AttachmentId,
                WeaponId);
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
            fields.Add("AttachmentWeaponId", AttachmentWeaponId);
            fields.Add("AttachmentId", AttachmentId);
            fields.Add("WeaponId", WeaponId);

            // return the insert statement
            return Insert.create("AttachmentWeapon", dbType, fields);
        }
    }
}
