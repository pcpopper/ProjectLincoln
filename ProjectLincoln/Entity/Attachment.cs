using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Attachment {

        #region Properties
        public int? AttachmentId { get; private set; } // The property for the  table's id
        public string AttachmentSerial { get; private set; } // The property for the foreign-key
        public int AttachmentTypeId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attachmentId">The attachment's id</param>
        /// <param name="attachmentSerial">The attachment's serial</param>
        /// <param name="attachmentTypeId">The attachment's type id</param>
        public Attachment (int? attachmentId, string attachmentSerial, int attachmentTypeId) {
            this.AttachmentId = attachmentId;
            this.AttachmentSerial = attachmentSerial;
            this.AttachmentTypeId = attachmentTypeId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Attachment getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new Attachment(
                reader.GetInt32("AttachmentId"),
                reader.GetString("AttachmentSerial"),
                reader.GetInt32("AttachmentTypeId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Attachment{{AttachmentId:{0}, AttachmentSerial:{1}, AttachmentTypeId:{2}}}",
                AttachmentId,
                AttachmentSerial,
                AttachmentTypeId);
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
            fields.Add("AttachmentId", AttachmentId);
            fields.Add("AttachmentSerial", AttachmentSerial);
            fields.Add("AttachmentTypeId", AttachmentTypeId);

            // return the insert statement
            return Insert.create("Attachment", dbType, fields);
        }
    }
}
