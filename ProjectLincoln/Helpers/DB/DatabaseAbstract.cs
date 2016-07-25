using System;

using ProjectLincoln.Enums;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Helpers.DB {
    public abstract class DatabaseAbstract {

        protected Select select = null; // The select builder
        protected string connectionString = ""; // The database's connection string
        protected string dbPath = ""; // The local database's connection path

        /// <summary>
        /// Builds all of the needed database objects
        /// </summary>
        /// <param name="type">The DatabaseType enum value of the database type</param>
        protected void buildDbObjects (DatabaseType type = DatabaseType.MySql) {
            switch (type) {
                case DatabaseType.MySql:
                    // Populate the connection string
                    connectionString = String.Format(
                        "server={0};uid={1};pwd={2};database={3};Allow User Variables=True",
                        Settings.Default.DBServer,
                        Settings.Default.DBUsername,
                        Settings.Default.DBPass,
                        Settings.Default.DBName);
                    break;
                case DatabaseType.SQLite:
                    // Create the database path
                    dbPath = System.IO.Path.Combine(
                        Environment.CurrentDirectory,
                        Settings.Default.DBFile);

                    // Initialize the connection string
                    if (Settings.Default.DevMode) {
                        connectionString = String.Format(
                            "data source={0};Version=3",
                            dbPath);
                    } else {
                        connectionString = String.Format(
                             "data source={0};Password={1};Version=3",
                             dbPath,
                             Settings.Default.DBPass);
                    }
                    break;
            }
        }
    }
}
