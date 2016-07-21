using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class AttachmentTruck {

        #region Properties
        public int? AttachmentTruckId { get; private set; } // The property for the  table's id
        public int AttachmentId { get; private set; } // The property for the foreign-key
        public int TruckId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attachmentTruckId">The attachmentTruck's id</param>
        /// <param name="attachmentId">The attachmentTruck's attachment id</param>
        /// <param name="truckId">The attachmentTruck's truck id</param>
        public AttachmentTruck (int? attachmentTruckId, int attachmentId, int truckId) {
            this.AttachmentTruckId = attachmentTruckId;
            this.AttachmentId = attachmentId;
            this.TruckId = truckId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static AttachmentTruck getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new AttachmentTruck(
                reader.GetInt32("AttachmentTruckId"),
                reader.GetInt32("AttachmentId"),
                reader.GetInt32("TruckId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "AttachmentTruck{{AttachmentTruckId:{0}, AttachmentId:{1}, TruckId:{2}}}",
                AttachmentTruckId,
                AttachmentId,
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
            fields.Add("AttachmentTruckId", AttachmentTruckId);
            fields.Add("AttachmentId", AttachmentId);
            fields.Add("TruckId", TruckId);

            // return the insert statement
            return Insert.create("AttachmentTruck", dbType, fields);
        }
    }
}
