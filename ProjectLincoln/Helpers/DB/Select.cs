using System;
using System.Collections.Generic;
using System.Linq;

using ProjectLincoln.Enums;

namespace ProjectLincoln.Helpers.DB {
    public class Select {

        private DatabaseType dbType = DatabaseType.MySql; // The type of databae
        private string table = ""; // The main table's name
        private Dictionary<string, string> aliases = null; // The alias' for the tables
        private Dictionary<string, List<string>> tableColumns = null; // The columns to show
        private List<string> joins = null, // The tables to join
            whereClauses = null, // The where clauses
            orderClauses = null; // The order by clauses

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table">The main table to select from</param>
        /// <param name="alias">Optional: The alias of the main table</param>
        /// <param name="columns">Optional: The columns to show</param>
        public Select (string table, string alias = null, List<string> columns = null) {
            // Initialize the arrays
            aliases = new Dictionary<string, string>();
            tableColumns = new Dictionary<string, List<string>>();
            joins = new List<string>();
            whereClauses = new List<string>();
            orderClauses = new List<string>();

            // Set the table
            this.table = (alias != null && !alias.Equals("")) ? table + " " + alias : table;

            // Check if the alias has been provided and that it is not blank
            if (alias != null && !alias.Equals("")) {
                // Add it to the list of aliases
                aliases.Add(table, alias);
            }

            // Check if there are any columns
            string tableAlias = getAlias(table, alias);
            if (columns != null && columns.Count() > 0) {
                // Add them to the column list
                tableColumns.Add(tableAlias, columns);
            } else {
                // Add a '*' to the column list
                tableColumns.Add(tableAlias, new List<string>() { "*" });
            }
        }

        /// <summary>
        /// Static Constructor
        /// </summary>
        /// <param name="table">The main table to select from</param>
        /// <param name="alias">Optional: The alias of the main table</param>
        /// <param name="columns">Optional: The columns to show</param>
        /// <returns>This class</returns>
        public static Select create (string table, string alias = null, List<string> columns = null) {
            return new Select(table, alias, columns);
        }

        /// <summary>
        /// Sets the database type if not MySQL
        /// </summary>
        /// <param name="dbType">The database type to change to</param>
        /// <returns>This class</returns>
        public Select setDB (DatabaseType dbType) {
            // Set the database type
            this.dbType = dbType;

            // create the chainable object
            return this;
        }

        /// <summary>
        /// Joins a table 
        /// </summary>
        /// <param name="table">The table to join</param>
        /// <param name="alias">Optional: The alias of the joined table</param>
        /// <param name="condition">Optional: The condition to use</param>
        /// <param name="type">Optional: Join type</param>
        /// <param name="columns">Optional: The columns to show</param>
        /// <returns>This class</returns>
        public Select join (string table, string alias = null, string condition = null, JoinType type = JoinType.Join, List<string> columns = null) {
            // Get the table's prefix
            string prefix = (alias != null && !alias.Equals("")) ? " " + alias : "";

            // Set the table
            string tableName = (alias != null && !alias.Equals("")) ? table + " " + alias : table;

            // Setup the join string
            string joinString = String.Format("JOIN {0}", tableName);

            // Add the condition clause if it is provided
            if (condition != null) {
                joinString = String.Format("{0} ON ({1})", joinString, condition);
            }

            // Set the correct join clause
            string clause = "";
            switch (type) {
                case JoinType.Inner:
                    clause = "INNER ";
                    break;
                case JoinType.Left:
                    clause = "LEFT ";
                    break;
                case JoinType.Right:
                    clause = "RIGHT ";
                    break;
            }

            // Add the join to the array
            joins.Add(String.Format("{0}{1}", clause, joinString));

            // Check if there are any columns
            if (columns != null && columns.Count() > 0) {
                // Add them to the column list
                tableColumns.Add(getAlias(table, alias), columns);
            }

            // create the chainable object
            return this;
        }

        /// <summary>
        /// Adds a where clause to the querystring
        /// </summary>
        /// <param name="clause">The where clause</param>
        /// <param name="type">The join type</param>
        /// <returns>This class</returns>
        public Select where (string clause, WhereType type = WhereType.AND) {
            // Check if there are any other where clauses
            if (whereClauses.Count() > 0) {
                whereClauses.Add(string.Format("{0} {1}", type.ToString(), clause));
            } else {
                whereClauses.Add(string.Format("{0}", clause));
            }

            // create the chainable object
            return this;
        }

        /// <summary>
        /// Adds a where clause to the querystring and replaces '??' with the ids
        /// </summary>
        /// <param name="clause">The where clause</param>
        /// <param name="ids">The integer list of ids to add</param>
        /// <param name="type">The join type</param>
        /// <returns>This class</returns>
        public Select where (string clause, List<int> ids, WhereType type = WhereType.AND) {
            // Replace the question marks with the comma seperated ids
            clause = clause.Replace("??", string.Join(", ", ids));

            // Return the where clause
            return where(clause, type);
        }

        /// <summary>
        /// Adds an order-by clause to the querystring
        /// </summary>
        /// <param name="column">The column to order by</param>
        /// <param name="type">The order to rows by</param>
        /// <returns>This class</returns>
        public Select order (string column, OrderType? type = null) {
            // Add the order-by clause
            switch (type) {
                case OrderType.ASC:
                case OrderType.DESC:
                    orderClauses.Add(string.Format("{0} {1}", column, type.ToString()));
                    break;
                default:
                    orderClauses.Add(string.Format("{0}", column));
                    break;
            }

            // Create the chainable object
            return this;
        }

        /// <summary>
        /// Adds non-prefixed columns to the querystring
        /// </summary>
        /// <param name="columns">The list of columns to add</param>
        /// <returns>This class</returns>
        public Select columns (List<string> columns) {
            // Add joined provided columns list to the columns array
            tableColumns.Add("none", columns);

            // create the chainable object
            return this;
        }

        /// <summary>
        /// Takes a list of columns, appends the prefix, and flattens to a string
        /// </summary>
        /// <param name="prefix">The table name or alias to prefix the columns with</param>
        /// <param name="columns">The list of columns</param>
        /// <returns></returns>
        private string processColumns (string prefix, List<string> columns) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the columns
            foreach (string column in columns) {
                // Add the concatenated strings to the output list
                output.Add(prefix + "." + column);
            }

            // Return the joined list
            return string.Join<string>(", ", output);
        }

        /// <summary>
        /// Processes and returns the appropriate alias
        /// </summary>
        /// <param name="table">The table's name</param>
        /// <param name="alias">Optional: The table's alias</param>
        /// <returns>The alias</returns>
        private string getAlias (string table, string alias = null) {
            return (alias != null && !alias.Equals("")) ? alias : table;
        }

        /// <summary>
        /// Override method that allows me to specify what is printed out
        /// </summary>
        /// <returns>The querystring</returns>
        public override string ToString () {
            // Return the formatted query string
            return ToInnerString() + ";";
        }

        /// <summary>
        /// Prints out the propper querystring - minus the semi-colon
        /// </summary>
        /// <returns>The querystring minus the semi-colon</returns>
        public string ToInnerString () {
            // Initialize the query string
            List<String> query = new List<string>();

            // Get the columns
            List<string> tmpCols = new List<string>();
            foreach (KeyValuePair<string, List<string>> table in tableColumns) {
                if (tableColumns.Count == 1 || table.Key.Equals("none")) {
                    tmpCols.Add(String.Join(", ", table.Value));
                } else if (tableColumns.Count > 1) {
                    tmpCols.Add(processColumns(table.Key, table.Value));
                }
            }
            string cols = (tableColumns.Count() > 0) ? string.Join<string>(", ", tmpCols) : "*";

            // Setup the query
            query.Add(string.Format("SELECT {0} FROM {1}", cols, table));

            // Check if there are joins
            if (joins.Count() > 0) {
                // Add the join clause(s)
                query.Add(string.Join<string>(" ", joins));
            }

            // Check if there are where clauses
            if (whereClauses.Count() > 0) {
                // Add the where clause(s)
                query.Add("WHERE " + string.Join(" ", whereClauses));
            }

            // Check if there are order-by clauses
            if (orderClauses.Count() > 0) {
                // Add the order-by clause(s)
                query.Add("ORDER BY " + string.Join(", ", orderClauses));
            }

            // Return the formatted query string
            return string.Join<string>(" ", query);
        }
    }

    public enum JoinType {
        Join, Inner, Left, Right
    }
    public enum OrderType {
        ASC, DESC
    }
    public enum WhereType {
        AND, OR
    }
}
