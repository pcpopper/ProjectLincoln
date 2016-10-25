using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Billet {

        #region Properties
        public int? BilletId { get; private set; } // The property for the table's id
        public string Code { get; private set; } // The property for the field
        public string Name { get; private set; } // The property for the field
        public string Short { get; private set; } // The property for the field
        public int ServiceId { get; private set; } // The property for the field
        public bool IsEnlisted { get; private set; } // The property for the field
        public bool IsWarrant { get; private set; } // The property for the field
        public bool IsOfficer { get; private set; } // The property for the field
        public int? LowRankId { get; private set; } // The property for the foreign-key
        public int? HighRankId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billetId">Optional: The billet's id</param>
        /// <param name="code">The billet's code</param>
        /// <param name="name">The billet's name</param>
        /// <param name="shortDesc">The billet's short description</param>
        /// <param name="serviceId">The billet's service id</param>
        /// <param name="isEnlisted">Is this billet for enlisted</param>
        /// <param name="isWarrant">Is this billet for warrant officer</param>
        /// <param name="isOfficer">Is this billet for officer</param>
        /// <param name="lowRankId">The lowest rank for this billet</param>
        /// <param name="highRankId">The highest rank for this billet</param>
        public Billet (int? billetId, string code, string name, string shortDesc, int serviceId, bool isEnlisted, bool isWarrant, bool isOfficer, int? lowRankId, int? highRankId) {
            this.BilletId = billetId;
            this.Code = code;
            this.Name = name;
            this.Short = shortDesc;
            this.ServiceId = serviceId;
            this.IsEnlisted = isEnlisted;
            this.IsWarrant = isWarrant;
            this.IsOfficer = isOfficer;
            this.LowRankId = lowRankId;
            this.HighRankId = highRankId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Billet getFromDb (MySqlDataReader reader) {
            #region Error checking
            // Error check the Code
            string code = null;
            if (!Convert.IsDBNull(reader["Code"])) {
                code = reader.GetString("Code");
            }
            // Error check the Short
            string shortDesc = null;
            if (!Convert.IsDBNull(reader["Short"])) {
                shortDesc = reader.GetString("Short");
            }
            // Error check the LowRankId
            int? lowRankId = null;
            if (!Convert.IsDBNull(reader["LowRankId"])) {
                lowRankId = reader.GetInt32("LowRankId");
            }
            // Error check the HighRankId
            int? highRankId = null;
            if (!Convert.IsDBNull(reader["HighRankId"])) {
                highRankId = reader.GetInt32("HighRankId");
            }
            #endregion

            // Return a new object with the provided values
            return new Billet(
                reader.GetInt32("BilletId"),
                code,
                reader.GetString("Name"),
                shortDesc,
                reader.GetInt32("ServiceId"),
                reader.GetBoolean("IsEnlisted"),
                reader.GetBoolean("IsWarrant"),
                reader.GetBoolean("IsOfficer"),
                lowRankId,
                highRankId);
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Billet{{BilletId:{0}, Code:{1}, Name: {2}, Short: {3}, ServiceId: {4}, IsEnlisted: {5}, IsWarrant: {6}, IsOfficer: {7}, LowRankId: {8}, HighRankId: {9}}}",
                BilletId,
                Code,
                Name,
                Short,
                ServiceId,
                IsEnlisted,
                IsWarrant,
                IsOfficer,
                LowRankId,
                HighRankId);
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
            fields.Add("BilletId", BilletId);
            fields.Add("Code", Code);
            fields.Add("Name", Name);
            fields.Add("Short", Short);
            fields.Add("ServiceId", ServiceId);
            fields.Add("IsEnlisted", IsEnlisted);
            fields.Add("IsWarrant", IsWarrant);
            fields.Add("IsOfficer", IsOfficer);
            fields.Add("LowRankId", LowRankId);
            fields.Add("HighRankId", HighRankId);

            // return the insert statement
            return Insert.create("Billet", dbType, fields);
        }
    }
}
