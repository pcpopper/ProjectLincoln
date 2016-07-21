using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class AttachmentWeaponService : ServiceAbstract {

        private List<AttachmentWeapon> joins = null;
        private List<Attachment> attachments = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public AttachmentWeaponService () {
            joins = new List<AttachmentWeapon>();
            attachments = new List<Attachment>();
        }

        /// <summary>
        /// Static constructor and retrieval of all attachments for the given weapons
        /// </summary>
        /// <param name="weaponIds">The list of weapon ids</param>
        /// <returns>New instance of this class</returns>
        public static AttachmentWeaponService retrieveAttachments (List<int> weaponIds) {
            // Create a new instance of this class
            AttachmentWeaponService awService = new AttachmentWeaponService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("AttachmentWeapon", "AW")
                    .join("Attachment", "A", "A.AttachmentId = AW.AttachmentId", JoinType.Inner)
                    .where("AW.WeaponId IN (??)", weaponIds);

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    awService.joins.Add(AttachmentWeapon.getFromDb(reader));
                    awService.attachments.Add(Attachment.getFromDb(reader));
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

            return awService;
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
