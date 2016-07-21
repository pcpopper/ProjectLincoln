using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Trailer {

        #region Properties
        public int? TrailerId { get; private set; } // The property for the table's id
        public string TrailerSerial { get; private set; } // The property for the field
        public int TamcnId { get; private set; } // The property for the foreign-key
        public int UnitId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trailerId">Optional: The trailer's id</param>
        /// <param name="trailerSerial">The trailer's serial</param>
        /// <param name="tamcnId">The trailer's tamcn</param>
        /// <param name="unitId">The unit that owns this trailer</param>
        public Trailer (int? trailerId, string trailerSerial, int tamcnId, int unitId) {
            this.TrailerId = trailerId;
            this.TrailerSerial = trailerSerial;
            this.TamcnId = tamcnId;
            this.UnitId = unitId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Trailer getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new Trailer(
                reader.GetInt32("TrailerId"),
                reader.GetString("TrailerSerial"),
                reader.GetInt32("TamcnId"),
                reader.GetInt32("UnitId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Trailer{{TrailerId:{0}, TrailerSerial:{1}, TamcnId:{2}, UnitId:{3}}}",
                TrailerId,
                TrailerSerial,
                TamcnId,
                UnitId);
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
            fields.Add("TrailerId", TrailerId);
            fields.Add("TrailerSerial", TrailerSerial);
            fields.Add("TamcnId", TamcnId);
            fields.Add("UnitId", UnitId);

            // return the insert statement
            return Insert.create("Trailer", dbType, fields);
        }
    }
}
