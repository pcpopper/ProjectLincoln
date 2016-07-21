using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Truck {

        #region Properties
        public int? TruckId { get; private set; } // The property for the table's id
        public string TruckSerial { get; private set; } // The property for the field
        public int PAX { get; private set; } // The property for the field
        public int TamcnId { get; private set; } // The property for the foreign-key
        public int? TrailerId { get; private set; } // The property for the foreign-key
        public int UnitId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="truckId">Optional: The trucks's id</param>
        /// <param name="truckSerial">The truck's serial</param>
        /// <param name="pax">The number of passengers for the truck</param>
        /// <param name="tamcnId">The truck's tamcn</param>
        /// <param name="trailerId">Optional: The TrailerId of the trailer attached to this truck</param>
        /// <param name="unitId">The unit that owns this truck</param>
        public Truck (int? truckId, string truckSerial, int pax, int tamcnId, int? trailerId, int unitId) {
            this.TruckId = truckId;
            this.TruckSerial = truckSerial;
            this.PAX = pax;
            this.TamcnId = tamcnId;
            this.TrailerId = trailerId;
            this.UnitId = unitId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Truck getFromDb (MySqlDataReader reader) {
            // Error check the TrailerId
            int? trailerId = null;
            if (!Convert.IsDBNull(reader["TrailerId"])) {
                trailerId = reader.GetInt32("TrailerId");
            }

            // Return a new object with the provided values
            return new Truck(
                reader.GetInt32("TruckId"),
                reader.GetString("TruckSerial"),
                reader.GetInt32("PAX"),
                reader.GetInt32("TamcnId"),
                trailerId,
                reader.GetInt32("UnitId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Truck{{TruckId:{0}, TruckSerial:{1}, PAX:{2}, TamcnId:{3}, TrailerId:{4}, UnitId:{5}}}",
                TruckId,
                TruckSerial,
                PAX,
                TamcnId,
                TrailerId,
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
            fields.Add("TruckId", TruckId);
            fields.Add("TruckSerial", TruckSerial);
            fields.Add("PAX", PAX);
            fields.Add("TamcnId", TamcnId);
            fields.Add("TrailerId", TrailerId);
            fields.Add("UnitId", UnitId);

            // return the insert statement
            return Insert.create("Truck", dbType, fields);
        }
    }
}
