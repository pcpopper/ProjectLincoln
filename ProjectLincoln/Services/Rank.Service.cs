using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Enums;
using ProjectLincoln.Entity;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class RankService : ServiceAbstract {

        private List<Rank> ranks = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public RankService () {
            ranks = new List<Rank>();
        }

        /// <summary>
        /// Static constructor and retrieval of all ranks
        /// </summary>
        /// <param name="order">Optional: The type of order-by clause to apply</param>
        /// <returns>New instance of this class</returns>
        public static RankService retrieveRanks (RankOrderBy order = RankOrderBy.Default) {
            // Create a new instance of this class
            RankService rService = new RankService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the querystring
                select = Select.create("Ranks");

                // Apply the order by clause
                switch (order) {
                    case RankOrderBy.Grade:
                        select.order(String.Format(
                            "{0}, {1}",
                            "FIELD(Grade, 'E-1', 'E-2', 'E-3', 'E-4', 'E-5', 'E-6', 'E-7', 'E-8', 'E-9', 'W-1', 'W-2', 'W-3', 'W-4', 'W-5', 'O-1', 'O-2', 'O-3', 'O-4', 'O-5', 'O-6', 'O-7', 'O-8', 'O-9', 'O-10')",
                            "RankId"));
                        break;
                    case RankOrderBy.Service:
                        select.order(String.Format(
                            "{0}, {1}, {2}",
                            "ServiceId ASC",
                            "FIELD(Grade, 'E-1', 'E-2', 'E-3', 'E-4', 'E-5', 'E-6', 'E-7', 'E-8', 'E-9', 'W-1', 'W-2', 'W-3', 'W-4', 'W-5', 'O-1', 'O-2', 'O-3', 'O-4', 'O-5', 'O-6', 'O-7', 'O-8', 'O-9', 'O-10')",
                            "RankId"));
                        break;
                }

                // Open the connection
                db = MySQL.openConnection(select);

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    rService.ranks.Add(Rank.getFromDb(reader));
                }

                // Close the connection and reader
                db.closeDB();
                #endregion
            } catch (Exception ex) when (ex is FormatException ) {
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

            return rService;
        }

        /// <summary>
        /// Returns a filtered list of the ranks
        /// </summary>
        /// <param name="gradeFilter">The selected range of grades to show</param>
        /// <param name="serviceId">The serviceId to filter by</param>
        /// <returns>The filtered list of ranks</returns>
        public List<Rank> getRanks (FilterGradesBy gradeFilter = FilterGradesBy.All, int? serviceId = null) {
            // Initialize the output
            List<Rank> output = new List<Rank>();

            // Check if there is any filter
            if (gradeFilter != FilterGradesBy.All || serviceId != null) {
                // Initialize the matches flag
                bool matches = false;

                // Loop through the units
                foreach (Rank rank in ranks) {
                    // Reset the matches flag
                    matches = false;

                    // Check if it matches the serviceFilter
                    if (serviceId != null && serviceId == rank.ServiceId) {
                        // Set the matches flag
                        matches = true;
                    }

                    // Choose what grade filter to apply
                    if (matches) {
                        switch (gradeFilter) {
                            case FilterGradesBy.Enlisted:
                                if (!rank.IsOfficer) { output.Add(rank); }
                                break;
                            case FilterGradesBy.Officer:
                                if (rank.IsOfficer && !rank.IsWarrant) { output.Add(rank); }
                                break;
                            case FilterGradesBy.Warrant:
                                if (rank.IsWarrant) { output.Add(rank); }
                                break;
                            case FilterGradesBy.WarrantAndElisted:
                                if (!rank.IsOfficer || rank.IsWarrant) { output.Add(rank); }
                                break;
                            case FilterGradesBy.WarrantAndOfficer:
                                if (rank.IsOfficer) { output.Add(rank); }
                                break;
                            default:
                                output.Add(rank);
                                break;
                        }
                    }
                }
            }

            // Return the filtered list of ranks
            return output;
        }

        /// <summary>
        /// Gets the insert statements for the list of ranks
        /// </summary>
        /// <param name="dbType">The database type</param>
        /// <returns>The list of insert statements</returns>
        public List<string> getInserts (DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the ranks
            foreach (Rank rank in ranks) {
                // Add the rank's insert statement
                output.Add(rank.ToInsert(dbType));
            }

            // Return the list of insert statements
            return output;
        }
    }

    public enum FilterGradesBy {
        Officer, Enlisted, Warrant, WarrantAndOfficer, WarrantAndElisted, All
    }
    public enum RankOrderBy {
        Default, Grade, Service
    }
}
