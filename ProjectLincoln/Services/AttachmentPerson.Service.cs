using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class AttachmentPersonService : ServiceAbstract {

        private List<AttachmentPerson> joins = null;
        private List<Attachment> attachments = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public AttachmentPersonService () {
            joins = new List<AttachmentPerson>();
            attachments = new List<Attachment>();
        }

        /// <summary>
        /// Static constructor and retrieval of all attachments for the given person
        /// </summary>
        /// <param name="personnelIds">The list of person ids</param>
        /// <returns>New instance of this class</returns>
        public static AttachmentPersonService retrieveAttachments (List<int> personnelIds) {
            // Create a new instance of this class
            AttachmentPersonService service = new AttachmentPersonService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("AttachmentPerson", "AP")
                    .join("Attachment", "A", "A.AttachmentId = AP.AttachmentId", JoinType.Inner)
                    .where("AP.PersonId IN (??)", personnelIds);

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    service.joins.Add(AttachmentPerson.getFromDb(reader));
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
                            "An error has occurred: {0}\n{1}",
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
