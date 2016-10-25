using System;
using System.Collections.Generic;

using ProjectLincoln.Enums;

namespace ProjectLincoln.Helpers.DB {
    public class Insert {
        private DatabaseType dbType = DatabaseType.MySql; // The type of database
        private string table = ""; // The table's name
        private Dictionary<string, Object> fields = null; // The fields to insert

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table">The table's name</param>
        /// <param name="fields">The fields to insert</param>
        public Insert (string table, DatabaseType dbType, Dictionary<string, Object> fields) {
            this.table = table;
            this.dbType = dbType;
            this.fields = fields;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <param name="table">The table's name</param>
        /// <param name="fields">The fields to insert</param>
        /// <returns>The insert statement</returns>
        public static string create (string table, DatabaseType dbType, Dictionary<string, Object> fields) {
            return new Insert(table, dbType, fields).ToString();
        }

        /// <summary>
        /// Override method that creates and returns an insert statement from the vars
        /// </summary>
        /// <returns>The string insert statement</returns>
        public override string ToString () {
            // Initialize the output
            List<string> output = new List<string>();

            // Initialize the temporary arrays
            List<string> columns = new List<string>();
            List<Object> values = new List<Object>();

            // Go through the fields and add them to their arrays
            foreach (KeyValuePair<string, Object> field in fields) {
                // Check if the value is null
                if (field.Value != null) {
                    // Add the key and value to their arrays
                    columns.Add(field.Key);

                    // Encapsulate anything other that int in single quotations
                    if (TypeChecker.IsNumeric(field.Value)) {
                        values.Add(field.Value);
                    } else if (TypeChecker.IsBoolean(field.Value)) {
                        values.Add(((bool) field.Value) ? 1 : 0);
                    } else {
                        values.Add("'" + field.Value.ToString() + "'");
                    }
                }
            }

            // Return the joined insert string
            return String.Format(
                "INSERT INTO {0} ({1}) VALUES ({2});",
                (dbType == DatabaseType.MySql) ? table : table.ToUpper(),
                String.Join(", ", columns),
                String.Join(", ", values));
        }
    }
}
