using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class AttachmentType {

        #region Properties
        public int? AttachmentTypeId { get; private set; } // The property for the table's id
        public int? TamcnId { get; private set; } // The property for the foreign-key
        public string Description { get; private set; } // The property for the field
        public bool IsPersonnel { get; private set; } // The property for the field
        public bool IsTruck { get; private set; } // The property for the field
        public bool IsWeapon { get; private set; } // The property for the field
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attachmentTypeId">The attachment type's id</param>
        /// <param name="tamcnId">Optional: The attachment type's tamcn</param>
        /// <param name="description">Optional: The attachment type's description</param>
        /// <param name="isPersonnel">Is this for a person?</param>
        /// <param name="isTruck">Is this for a truck?</param>
        /// <param name="isWeapon">Is this for a weapon?</param>
        public AttachmentType (int? attachmentTypeId, int? tamcnId, string description, bool isPersonnel, bool isTruck, bool isWeapon) {
            this.AttachmentTypeId = attachmentTypeId;
            this.TamcnId = tamcnId;
            this.Description = description;
            this.IsPersonnel = isPersonnel;
            this.IsTruck = isTruck;
            this.IsWeapon = isWeapon;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static AttachmentType getFromDb (MySqlDataReader reader) {
            // Error check the TamcnId
            int? tamcnId = null;
            if (!Convert.IsDBNull(reader["TamcnId"])) {
                tamcnId = reader.GetInt32("TamcnId");
            }
            // Error check the Description
            string description = null;
            if (!Convert.IsDBNull(reader["Description"])) {
                description = reader.GetString("Description");
            }

            // Return a new object with the provided values
            return new AttachmentType(
                reader.GetInt32("AttachmentTypeId"),
                tamcnId,
                description,
                reader.GetBoolean("IsPersonnel"),
                reader.GetBoolean("IsTruck"),
                reader.GetBoolean("IsWeapon"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "AttachmentType{{AttachmentTypeId:{0}, TamcnId:{1}, Description:{2}, IsPersonnel:{3}, IsTruck:{4}, IsWeapon:{5}}}",
                AttachmentTypeId,
                TamcnId,
                Description,
                IsPersonnel,
                IsTruck,
                IsWeapon);
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
            fields.Add("AttachmentTypeId", AttachmentTypeId);
            fields.Add("TamcnId", TamcnId);
            fields.Add("Description", Description);
            fields.Add("IsPersonnel", IsPersonnel);
            fields.Add("IsTruck", IsTruck);
            fields.Add("IsWeapon", IsWeapon);

            // return the insert statement
            return Insert.create("AttachmentType", dbType, fields);
        }
    }
}
