using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class AttachmentTruckService : ServiceAbstract {

        private List<AttachmentTruck> joins = null;
        private List<Attachment> attachments = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public AttachmentTruckService () {
            joins = new List<AttachmentTruck>();
            attachments = new List<Attachment>();
        }

        /// <summary>
        /// Static constructor and retrieval of all attachments for the given trucks
        /// </summary>
        /// <param name="truckIds">The list of truck ids</param>
        /// <returns>New instance of this class</returns>
        public static AttachmentTruckService retrieveAttachments (List<int> truckIds) {
            // Create a new instance of this class
            AttachmentTruckService service = new AttachmentTruckService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("AttachmentTruck", "AT")
                    .join("Attachment", "A", "A.AttachmentId = AT.AttachmentId", JoinType.Inner)
                    .where("AT.TruckId IN (??)", truckIds);

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    service.joins.Add(AttachmentTruck.getFromDb(reader));
                    service.attachments.Add(Attachment.getFromDb(reader));
                }

                // Close the connection and reader
                db.closeDB();
                #endregion
            } catch (Exception ex) when (ex is FormatException) {
                #region Catch
                // Try to close the connection and reader
                if (db != null) {
                    db.closeDB();
                }

                // Show the error message
                if (Settings.Default.DevMode) {
                    Debug.WriteLine(
                        String.Format(
                            "An error has occurred: {1}\n{2}",
                            ex.Message,
                            ex.StackTrace));
                }
                #endregion
            }

            return service;
        }

        /// <summary>
        /// Returns the list of attachments
        /// </summary>
        /// <returns>The list of attachments</returns>
        public List<Attachment> getAttachments () {
            return attachments;
        }

        /// <summary>
        /// Gets the insert statements for the list
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the two lists
            for (int i = 0; i < Math.Min(joins.Count, attachments.Count); i++) {
                // Add the insert statements
                output.Add(attachments[i].ToInsert(dbType));
                output.Add(joins[i].ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }
}
