using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Person {

        #region Properties
        public int? PersonId { get; private set; } // The property for the table's id
        public string Serial { get; private set; } // The property for the field
        public string FirstName { get; private set; } // The property for the field
        public string MiddleInitial { get; private set; } // The property for the field
        public string LastName { get; private set; } // The property for the field
        public BloodTypes? BloodType { get; private set; } // The property for the field
        public Genders? Sex { get; private set; } // The property for the field
        public int BilletId { get; private set; } // The property for the foreign-key
        public int UnitId { get; private set; } // The property for the foreign-key
        public int? RankId { get; private set; } // The property for the foreign-key
        public int ServiceId { get; private set; } // The property for the foreign-key
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="personId">Optional: The person's id</param>
        /// <param name="serial">The person's serial</param>
        /// <param name="firstName">The person's first name</param>
        /// <param name="middleInitial">The person's middle initial</param>
        /// <param name="lastName">The person's last name</param>
        /// <param name="bloodType">The person's blood type</param>
        /// <param name="sex">The person's gender</param>
        /// <param name="billetId">The person's billet id</param>
        /// <param name="unitId">The person's unit id</param>
        /// <param name="rankId">The person's rank id</param>
        /// <param name="serviceId">The person's service id</param>
        public Person (int? personId, string serial, string firstName, string middleInitial, string lastName, BloodTypes? bloodType, Genders sex, int billetId, int unitId, int? rankId, int serviceId) {
            this.PersonId = personId;
            this.Serial = serial;
            this.FirstName = firstName;
            this.MiddleInitial = middleInitial;
            this.LastName = lastName;
            this.BloodType = bloodType;
            this.Sex = sex;
            this.BilletId = billetId;
            this.UnitId = unitId;
            this.RankId = rankId;
            this.ServiceId = serviceId;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Person getFromDb (MySqlDataReader reader) {
            #region Error Checking
            string serial = (!Convert.IsDBNull(reader["Serial"])) ? reader.GetString("Serial") : null;
            string firstName = (!Convert.IsDBNull(reader["FirstName"])) ? reader.GetString("FirstName") : null;
            string middleInitial = (!Convert.IsDBNull(reader["MiddleInitial"])) ? reader.GetString("MiddleInitial") : null;
            BloodTypes? bloodType = (!Convert.IsDBNull(reader["BloodType"])) ? (BloodTypes?) EnumHelper.GetValueFromDescription<BloodTypes>(reader.GetString("BloodType")) : null;
            int? rankId = (!Convert.IsDBNull(reader["rankId"])) ? (int?) reader.GetInt32("rankId") : null;
            #endregion

            // Return a new object with the provided values
            return new Person(
                reader.GetInt32("PersonId"),
                serial,
                firstName,
                middleInitial,
                reader.GetString("LastName"),
                bloodType,
                EnumHelper.GetValueFromDescription<Genders>(reader.GetString("Sex")),
                reader.GetInt32("BilletId"),
                reader.GetInt32("UnitId"),
                rankId,
                reader.GetInt32("ServiceId"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Person{{PersonId:{0}, Serial:{1}, FirstName:{2}, MiddleInitial:{3}, LastName:{4}, BloodType:{5}, Sex: {6}, BilletId: {7}, UnitId: {8}, RankId: {9}, ServiceId: {10}}}",
                PersonId,
                Serial,
                FirstName,
                MiddleInitial,
                LastName,
                EnumHelper.GetEnumDescription(BloodType),
                EnumHelper.GetEnumDescription(Sex),
                BilletId,
                UnitId,
                RankId,
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
            fields.Add("PersonId", PersonId);
            fields.Add("Serial", Serial);
            fields.Add("FirstName", FirstName);
            fields.Add("MiddleInitial", MiddleInitial);
            fields.Add("LastName", LastName);
            fields.Add("BloodType", EnumHelper.GetEnumDescription(BloodType));
            fields.Add("Sex", EnumHelper.GetEnumDescription(Sex));
            fields.Add("BilletId", BilletId);
            fields.Add("UnitId", UnitId);
            fields.Add("RankId", RankId);
            fields.Add("ServiceId", ServiceId);

            // return the insert statement
            return Insert.create("Person", dbType, fields);
        }
    }

    public enum BloodTypes {
        [Description("A+")] APOS,
        [Description("A-")] ANEG,
        [Description("B+")] BPOS,
        [Description("B-")] BNEG,
        [Description("AB+")] ABPOS,
        [Description("AB-")] ABNEG,
        [Description("O+")] OPOS,
        [Description("O-")] ONEG
    }
    public enum Genders {
        M, F
    }
}
