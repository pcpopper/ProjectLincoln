using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Rank {

        #region Properties
        public int? RankId { get; private set; } // The property for the table's id
        public string Grade { get; private set; } // The property for the field
        public string Name { get; private set; } // The property for the field
        public string Abbreviation { get; private set; } // The property for the field
        public bool IsOfficer { get; private set; } // The property for the field
        public bool IsWarrant { get; private set; } // The property for the field
        public int ServiceId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rankId">The ranks's id</param>
        /// <param name="grade">The ranks's grade</param>
        /// <param name="name">The ranks's name</param>
        /// <param name="abbreviation">The ranks's abbreviation</param>
        /// <param name="isOfficer">The officer status of the ranks</param>
        /// <param name="isWarrant">The warrant officer status of the ranks</param>
        /// <param name="serviceId">The service that the ranks belongs to</param>
        public Rank (int? rankId, string grade, string name, string abbreviation, bool isOfficer, bool isWarrant, int serviceId) {
            this.RankId = rankId;
            this.Grade = grade;
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.IsOfficer = isOfficer;
            this.IsWarrant = IsWarrant;
            this.ServiceId = serviceId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Rank getFromDb (MySqlDataReader reader) {
            // Return a new object with the provided values
            return new Rank(
                reader.GetInt32("RankId"),
                reader.GetString("Grade"),
                reader.GetString("Name"),
                reader.GetString("Abbreviation"),
                reader.GetBoolean("IsOfficer"),
                reader.GetBoolean("IsWarrant"),
                reader.GetInt32("ServiceId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Rank{{RankId:{0}, Grade:{1}, Name:{2}, Abbreviation:{3}, IsOfficer:{4}, IsWarrant:{5}, ServiceId:{6}}}",
                RankId,
                Grade,
                Name,
                Abbreviation,
                IsOfficer,
                IsWarrant,
                ServiceId);
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
            fields.Add("RankId", RankId);
            fields.Add("Name", Name);
            fields.Add("Grade", Grade);
            fields.Add("Abbreviation", Abbreviation);
            fields.Add("IsOfficer", IsOfficer);
            fields.Add("IsWarrant", IsWarrant);
            fields.Add("ServiceId", ServiceId);

            // Return the insert statement
            return Insert.create("Ranks", dbType, fields);
        }
    }
}
