using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class AttachmentPerson {

        #region Properties
        public int? AttachmentPersonId { get; private set; } // The property for the  table's id
        public int AttachmentId { get; private set; } // The property for the foreign-key
        public int PersonId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attachmentPersonId">The attachmentPerson's id</param>
        /// <param name="attachmentId">The attachmentPerson's attachment id</param>
        /// <param name="personId">The attachmentPerson's person id</param>
        public AttachmentPerson (int? attachmentPersonId, int attachmentId, int personId) {
            this.AttachmentPersonId = attachmentPersonId;
            this.AttachmentId = attachmentId;
            this.PersonId = personId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static AttachmentPerson getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new AttachmentPerson(
                reader.GetInt32("AttachmentPersonId"),
                reader.GetInt32("AttachmentId"),
                reader.GetInt32("PersonId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "AttachmentPerson{{AttachmentPersonId:{0}, AttachmentId:{1}, PersonId:{2}}}",
                AttachmentPersonId,
                AttachmentId,
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
            fields.Add("AttachmentPersonId", AttachmentPersonId);
            fields.Add("AttachmentId", AttachmentId);
            fields.Add("PersonId", PersonId);

            // return the insert statement
            return Insert.create("AttachmentPerson", dbType, fields);
        }
    }
}
