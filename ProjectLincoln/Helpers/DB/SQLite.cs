using System;
using System.Collections.Generic;
using System.Data.SQLite;

using ProjectLincoln.Enums;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Helpers.DB {
    public class SQLite : DatabaseAbstract {
        public bool isCreated = false; // Flag that tells the loading sequence that the db is finished being created

        private SQLiteConnection conn = null; // The database connection
        private SQLiteCommand comm = null; // The database command object

        private Splash splash = null; // The local reference to the splash screen

        /// <summary>
        /// Constructor
        /// </summary>
        public SQLite () {
            // Initialize all of the database objects
            buildDbObjects(DatabaseType.SQLite);
        }

        /// <summary>
        /// Sets the local reference to the splash screen
        /// </summary>
        /// <param name="splash">The reference to the splash screen</param>
        public void setSplash (Splash splash) {
            this.splash = splash;
        }

        /// <summary>
        /// Method that runs the sql queries in order to create the database structure
        /// </summary>
        public void createDatabase () {
            // Delete the db if in dev mode
            if (Settings.Default.DevMode) {
                try {
                    System.IO.File.Delete(dbPath);
                } catch (Exception ex) {
                    // Do nothing with the exception
                }
            }

            // Check if the db exists
            if (!System.IO.File.Exists(dbPath)) {
                // Create the database file
                SQLiteConnection.CreateFile(dbPath);
            }

            // Connect to the db
            using (conn = new SQLiteConnection(connectionString)) {
                try {
                    #region Try
                    // Try to connect
                    conn.Open();

                    // Initialize the command object
                    using (comm = new SQLiteCommand(conn)) {
                        // Create the database structure if this is the first time
                        if (!isCreated) {

                            #region Drop table statements
                            // Setup the sql statements for dropping the tables
                            Dictionary<string, string> dropTables = new Dictionary<string, string>() {
                                { "Tamcn", @"DROP TABLE IF EXISTS Tamcn;" },
                                { "WeaponType", @"DROP TABLE IF EXISTS WeaponType;" },
                                { "Weapon", @"DROP TABLE IF EXISTS Weapon;" },
                                { "AttachmentType", @"DROP TABLE IF EXISTS AttachmentType;" },
                                { "Attachment", @"DROP TABLE IF EXISTS Attachment;" },
                                { "Services", @"DROP TABLE IF EXISTS Services;" },
                                { "UnitLevel", @"DROP TABLE IF EXISTS UnitLevel;" },
                                { "Unit", @"DROP TABLE IF EXISTS Unit;" },
                                { "Trailer", @"DROP TABLE IF EXISTS Trailer;" },
                                { "Truck", @"DROP TABLE IF EXISTS Truck;" },
                                { "WeaponTruck", @"DROP TABLE IF EXISTS WeaponTruck;" },
                                { "Ranks", @"DROP TABLE IF EXISTS Ranks;" },
                                { "Billet", @"DROP TABLE IF EXISTS Billet;" },
                                { "Person", @"DROP TABLE IF EXISTS Person;" },
                                { "WeaponPerson", @"DROP TABLE IF EXISTS WeaponPerson;" },
                                { "AttachmentTruck", @"DROP TABLE IF EXISTS AttachmentTruck;" },
                                { "AttachmentWeapon", @"DROP TABLE IF EXISTS AttachmentWeapon;" },
                                { "AttachmentPerson", @"DROP TABLE IF EXISTS AttachmentPerson;" },
                                { "Users", @"DROP TABLE IF EXISTS Users;" },
                                { "Pass", @"DROP TABLE IF EXISTS Pass;" },
                                { "UnitTree", @"DROP TABLE IF EXISTS UnitTree;" },
                                { "Manifests", @"DROP TABLE IF EXISTS Manifests;" }
                            };
                            #endregion

                            #region Create table statements
                            // Setup the sql statements for creating the tables
                            Dictionary<string, string> createTables = new Dictionary<string, string>() {
                                { "Tamcn", @"CREATE TABLE IF NOT EXISTS Tamcn (
                                  TamcnId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Tamcn TEXT NOT NULL,
                                  Nomen TEXT NOT NULL,
                                  Short TEXT NOT NULL,
                                  PAX INTEGER NULL,
                                  IsTruck INTEGER NOT NULL DEFAULT 0,
                                  IsTrailer INTEGER NOT NULL DEFAULT 0,
                                  IsWeapon INTEGER NOT NULL DEFAULT 0,
                                  IsAttachment INTEGER NOT NULL DEFAULT 0);" },
                                { "WeaponType", @"CREATE TABLE IF NOT EXISTS WeaponType (
                                  WeaponTypeId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  TamcnId INTEGER NOT NULL,
                                  IsPersonnel INTEGER NOT NULL DEFAULT 0,
                                  IsTruck INTEGER NOT NULL DEFAULT 0);" },
                                { "Weapon", @"CREATE TABLE IF NOT EXISTS Weapon (
                                  WeaponId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  WeaponSerial TEXT NOT NULL,
                                  WeaponTypeId INTEGER NOT NULL);" },
                                { "AttachmentType", @"CREATE TABLE IF NOT EXISTS AttachmentType (
                                  AttachmentTypeId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  TamcnId INTEGER NULL,
                                  Description TEXT NULL,
                                  IsPersonnel INTEGER NOT NULL DEFAULT 0,
                                  IsTruck INTEGER NOT NULL DEFAULT 0,
                                  IsWeapon INTEGER NOT NULL DEFAULT 0);" },
                                { "Attachment", @"CREATE TABLE IF NOT EXISTS Attachment (
                                  AttachmentId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  AttachmentSerial TEXT NOT NULL,
                                  AttachmentTypeId INTEGER NOT NULL);" },
                                { "Services", @"CREATE TABLE IF NOT EXISTS Services (
                                  ServiceId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Name TEXT NULL,
                                  Abbreviation TEXT NOT NULL);" },
                                { "UnitLevel", @"CREATE TABLE IF NOT EXISTS UnitLevel (
                                  UnitLevelId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Level INTEGER NOT NULL,
                                  Name TEXT NOT NULL,
                                  Short TEXT NOT NULL,
                                  ServiceId INTEGER NOT NULL);" },
                                { "Unit", @"CREATE TABLE IF NOT EXISTS Unit (
                                  UnitId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Name TEXT NULL,
                                  Abbreviation TEXT NOT NULL,
                                  UnitLevelId INTEGER NULL,
                                  ServiceId INTEGER NULL,
                                  Updated DATE NOT NULL);" },
                                { "Trailer", @"CREATE TABLE IF NOT EXISTS Trailer (
                                  TrailerId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  TrailerSerial TEXT NOT NULL,
                                  TamcnId INTEGER NOT NULL,
                                  UnitId INTEGER NOT NULL);" },
                                { "Truck", @"CREATE TABLE IF NOT EXISTS Truck (
                                  TruckId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  TruckSerial TEXT NOT NULL,
                                  PAX INTEGER NOT NULL,
                                  TamcnId INTEGER NOT NULL,
                                  TrailerId INTEGER NULL,
                                  UnitId INTEGER NOT NULL);" },
                                { "WeaponTruck", @"CREATE TABLE IF NOT EXISTS WeaponTruck (
                                  WeaponTruckId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  WeaponId INTEGER NOT NULL,
                                  TruckId INTEGER NOT NULL);" },
                                { "Ranks", @"CREATE TABLE IF NOT EXISTS Ranks (
                                  RankId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Grade TEXT NOT NULL,
                                  Name TEXT NOT NULL,
                                  Abbreviation TEXT NOT NULL,
                                  IsOfficer INTEGER NOT NULL DEFAULT 0,
                                  IsWarrant INTEGER NOT NULL DEFAULT 0,
                                  ServiceId INTEGER NOT NULL);" },
                                { "Billet", @"CREATE TABLE IF NOT EXISTS Billet (
                                  BilletId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Code TEXT NULL,
                                  Name TEXT NOT NULL,
                                  Short TEXT NULL,
                                  ServiceId INTEGER NOT NULL,
                                  IsEnlisted INTEGER NOT NULL DEFAULT 0,
                                  IsWarrant INTEGER NOT NULL DEFAULT 0,
                                  IsOfficer INTEGER NOT NULL DEFAULT 0,
                                  LowRankId INTEGER NULL,
                                  HighRankId INTEGER NULL);" },
                                { "Person", @"CREATE TABLE IF NOT EXISTS Person (
                                  PersonId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Serial TEXT NULL,
                                  FirstName TEXT NULL,
                                  MiddleInitial TEXT NULL,
                                  LastName TEXT NOT NULL,
                                  BloodType TEXT NULL,
                                  Sex TEXT NOT NULL,
                                  BilletId INTEGER NOT NULL,
                                  UnitId INTEGER NOT NULL,
                                  RankId INTEGER NULL,
                                  ServiceId INTEGER NOT NULL);" },
                                { "WeaponPerson", @"CREATE TABLE IF NOT EXISTS WeaponPerson (
                                  WeaponPersonId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  WeaponId INTEGER NOT NULL,
                                  PersonId INTEGER NOT NULL);" },
                                { "AttachmentTruck", @"CREATE TABLE IF NOT EXISTS AttachmentTruck (
                                  AttachmentTruckId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  AttachmentId INTEGER NOT NULL,
                                  TruckId INTEGER NOT NULL);" },
                                { "AttachmentWeapon", @"CREATE TABLE IF NOT EXISTS AttachmentWeapon (
                                  AttachmentWeaponId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  AttachmentId INTEGER NOT NULL,
                                  WeaponId INTEGER NOT NULL);" },
                                { "AttachmentPerson", @"CREATE TABLE IF NOT EXISTS AttachmentPerson (
                                  AttachmentPersonID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  AttachmentId INTEGER NOT NULL,
                                  PersonId INTEGER NOT NULL);" },
                                { "Users", @"CREATE TABLE IF NOT EXISTS Users (
                                  UserId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  Username TEXT NOT NULL,
                                  LastUpdated DATE NULL,
                                  LastLogin DATE NULL,
                                  Activated INTEGER NULL DEFAULT 0,
                                  UnitId INTEGER NULL,
                                  IsDevAdmin INTEGER NULL DEFAULT 0,
                                  IsUnitAdmin INTEGER NULL DEFAULT 0,
                                  IsUserAdmin INTEGER NULL DEFAULT 0);" },
                                { "Pass", @"CREATE TABLE IF NOT EXISTS Pass (
                                  UserId INTEGER NOT NULL,
                                  Pass TEXT NOT NULL,
                                  Salt TEXT NOT NULL);" },
                                { "UnitTree", @"CREATE TABLE IF NOT EXISTS UnitTree (
                                  UnitTreeId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  UnitId INTEGER NOT NULL,
                                  ParentId INTEGER NOT NULL);" },
                                { "Manifests", @"CREATE TABLE IF NOT EXISTS Manifests (
                                  ManifestId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                  CreatedOn DATE NOT NULL,
                                  CreatedBy INTEGER NOT NULL,
                                  EditedOn DATE NULL,
                                  EditedBy INTEGER NULL,
                                  JSONBlob TEXT NULL);" }
                            };
                            #endregion

                            #region Drop tables
                            // Run the queries to drop the tables
                            foreach (KeyValuePair<string, string> table in dropTables) {
                                // Show the new message
                                splash.updateLoadingMessage(string.Format("Dropping the local table {0}", table.Key));

                                // Set the command text
                                comm.CommandText = table.Value;

                                // Execute the command
                                comm.ExecuteNonQuery();
                            }
                            #endregion

                            #region Create tables
                            // Run the queries to create the tables
                            foreach (KeyValuePair<string, string> table in createTables) {
                                // Show the new message
                                splash.updateLoadingMessage(string.Format("Creating the local table {0}", table.Key));

                                // Set the command text
                                comm.CommandText = table.Value;

                                // Execute the command
                                comm.ExecuteNonQuery();
                            }
                            #endregion
                        }

                        // Close the db
                        conn.Close();

                        // Set the created flag
                        isCreated = true;
                    }
                    #endregion
                } catch (Exception ex) {
                    #region Catch
                    // Close the connection
                    conn.Close();

                    // Show the error message
                    if (Settings.Default.DevMode) {
                        System.Diagnostics.Debug.WriteLine(
                            string.Format(
                                "An error has occurred: {0}\n{1}",
                                ex.Message,
                                ex.StackTrace));
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Inserts all of the provided data into the local db
        /// </summary>
        /// <param name="syncData">The list of string insert statements</param>
        public void syncData (List<string> syncData) {
            // Connect to the db
            using (conn = new SQLiteConnection(connectionString)) {
                try {
                    #region Try
                    // Try to connect
                    conn.Open();

                    // Initialize the command object
                    using (comm = new SQLiteCommand(conn)) {
                        // Run the queries to drop the tables
                        for (int i = 0; i < syncData.Count; i++) {
                            // Show the new message
                            splash.updateLoadingMessage(string.Format("Inserting row {0} of {1}.", i, syncData.Count));

                            // Set the command text
                            comm.CommandText = syncData[i];

                            // Execute the command
                            comm.ExecuteNonQuery();
                        }

                        // Close the db
                        conn.Close();

                        // Set the created flag
                        isCreated = true;
                    }
                    #endregion
                } catch (Exception ex) {
                    #region Catch
                    // Close the connection
                    conn.Close();

                    // Show the error message
                    if (Settings.Default.DevMode) {
                        System.Diagnostics.Debug.WriteLine(
                            string.Format(
                                "An error has occurred: {0}\n{1}",
                                ex.Message,
                                ex.StackTrace));
                    }
                    #endregion
                }
            }
        }
    }
}
