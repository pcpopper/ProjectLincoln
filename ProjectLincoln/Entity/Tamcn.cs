using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln.Entity {
    public class Tamcn {

        #region Properties
        public int? TamcnId { get; private set; } // The property for the table's id
        public string TamcnCode { get; private set; } // The property for the field
        public string Nomen { get; private set; } // The property for the field
        public string ShortDesc { get; private set; } // The property for the field
        public int? Pax { get; private set; } // The property for the field
        public bool IsTruck { get; private set; } // The property for the field
        public bool IsTrailer { get; private set; } // The property for the field
        public bool IsWeapon { get; private set; } // The property for the field
        public bool IsAttachment { get; private set; } // The property for the field
        public string ImageName { get; private set; } // The property for the field
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tamcnId">Optional: The tamcn's id</param>
        /// <param name="tamcnCode">The tamcn code</param>
        /// <param name="nomen">The nomenclature</param>
        /// <param name="shortDesc">The short description</param>
        /// <param name="pax">Optional: The number of passengers</param>
        /// <param name="isTruck">Is this a truck?</param>
        /// <param name="isTrailer">Is this a trailer?</param>
        /// <param name="isWeapon">Is this a weapon?</param>
        /// <param name="isAttachment">Is this a attachment?</param>
        /// <param name="imageName">The image name for the icon</param>
        public Tamcn (int? tamcnId, string tamcnCode, string nomen, string shortDesc, int? pax, bool isTruck,
            bool isTrailer, bool isWeapon, bool isAttachment, string imageName) {
            this.TamcnId = tamcnId;
            this.TamcnCode = tamcnCode;
            this.Nomen = nomen;
            this.ShortDesc = shortDesc;
            this.Pax = pax;
            this.IsTruck = isTruck;
            this.IsTrailer = isTrailer;
            this.IsWeapon = isWeapon;
            this.IsAttachment = isAttachment;
            this.ImageName = imageName;
        }

        /// <summary>
        /// Creates and returns a new object with the information from the db reader
        /// </summary>
        /// <param name="reader">The record from the db</param>
        /// <returns>An instantiated version of this class</returns>
        public static Tamcn getFromDb (MySqlDataReader reader) {
            // Error check the Pax
            int? pax = null;
            if (!Convert.IsDBNull(reader["Pax"])) {
                pax = reader.GetInt32("Pax");
            }

            // Return a new object with the provided values
            return new Tamcn(
                reader.GetInt32("TamcnId"),
                reader.GetString("Tamcn"),
                reader.GetString("Nomen"),
                reader.GetString("Short"),
                pax,
                reader.GetBoolean("IsTruck"),
                reader.GetBoolean("IsTrailer"),
                reader.GetBoolean("IsWeapon"),
                reader.GetBoolean("IsAttachment"),
                reader.GetString("ImageName"));
        }

        /// <summary>
        /// Override function that provides the format that is wanted
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString () {
            return string.Format(
                "Tamcn{{TamcnId:{0}, Tamcn:{1}, Nomen:{2}, Short:{3}, PAX:{4}, IsTruck:{5}, IsTrailer:{6}, IsWeapon:{7}, IsAttachment:{8}, ImageName:{9}}}",
                TamcnId,
                TamcnCode,
                Nomen,
                ShortDesc,
                Pax,
                IsTruck,
                IsTrailer,
                IsWeapon,
                IsAttachment,
                ImageName);
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
            fields.Add("TamcnId", TamcnId);
            fields.Add("Tamcn", TamcnCode);
            fields.Add("Nomen", Nomen);
            fields.Add("Short", ShortDesc);
            fields.Add("PAX", Pax);
            fields.Add("IsTruck", IsTruck);
            fields.Add("IsTrailer", IsTrailer);
            fields.Add("IsWeapon", IsWeapon);
            fields.Add("IsAttachment", IsAttachment);
            fields.Add("ImageName", ImageName);

            // return the insert statement
            return Insert.create("Tamcn", dbType, fields);
        }
    }
}
