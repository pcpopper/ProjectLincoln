using System;
using System.Collections.Generic;
using System.Data.SQLite;

using ProjectLincoln.Enums;

namespace ProjectLincoln.Helpers.DB {
    public class SQLite : DatabaseAbstract {
        public bool isCreated = false; // Flag that tells the loading sequence that the db is finished being created

        private SQLiteConnection conn = null; // The database connection
        private SQLiteCommand comm = null; // The database command object

        /// <summary>
        /// Constructor
        /// </summary>
        public SQLite () {
            // Initialize all of the database objects
            buildDbObjects(DatabaseType.SQLite);
        }

        /// <summary>
        /// Method that runs the sql queries in order to create the database structure
        /// </summary>
        public void createDatabase () {
            // Check if the db exists
            if (!System.IO.File.Exists(dbPath)) {
                // Create the database file
                SQLiteConnection.CreateFile(dbPath);
            }

            // Connect to the db
            using (conn = new SQLiteConnection(connectionString)) {
                try {
                    // Try to connect
                    conn.Open();

                    // Initialize the command object
                    using (comm = new SQLiteCommand(conn)) {
                        // Create the database structure if this is the first time
                        if (isCreated) {
                            // Setup the sql statements for the tables
                            String[] tables = {
                                @"CREATE TABLE IF NOT EXISTS WeaponType (
                                  WeaponTypeId INTEGER,
                                  NOMEN TEXT)",
                                @"CREATE TABLE IF NOT EXISTS Weapon (
                                  WeaponSerial TEXT,
                                  WeaponTypeId INTEGER,
                                  Assignable TEXT)",
                                @"CREATE TABLE IF NOT EXISTS AttachmentType (
                                  AttachmentTypeId INTEGER,
                                  NOMEN TEXT,
                                  Assignable TEXT)",
                                @"CREATE TABLE IF NOT EXISTS Attachment (
                                  AttachmentId INTEGER,
                                  AttachmentSerial TEXT,
                                  AttachmentTypeId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS Tamnc (
                                  Tamnc TEXT,
                                  NOMEN TEXT,
                                  PAX INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS Trailer (
                                  TrailerId INTEGER,
                                  TrailerSerial TEXT,
                                  Tamnc TEXT)",
                                @"CREATE TABLE IF NOT EXISTS Truck (
                                  TruckId INTEGER,
                                  TruckSerial TEXT,
                                  PAX INTEGER,
                                  Tamnc TEXT,
                                  TrailerId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS WeaponTruck (
                                  WeaponSerial TEXT,
                                  TruckId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS Services (
                                  ServiceId INTEGER,
                                  Name TEXT,
                                  Abbreviation TEXT)",
                                @"CREATE TABLE IF NOT EXISTS Unit (
                                  UnitId INTEGER,
                                  Name TEXT,
                                  Abbreviation TEXT,
                                  ServiceId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS Ranks (
                                  RankId INTEGER,
                                  Grade TEXT,
                                  Name TEXT,
                                  Abbreviation TEXT,
                                  IsOfficer INTEGER,
                                  ServiceId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS Person (
                                  PersonId INTEGER,
                                  Serial TEXT,
                                  FirstName TEXT,
                                  MiddleInitial TEXT,
                                  LastName TEXT,
                                  BloodType TEXT,
                                  Sex TEXT,
                                  MOS TEXT,
                                  UnitId INTEGER,
                                  RankId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS WeaponPerson (
                                  WeaponPersonId INTEGER,
                                  WeaponSerial TEXT,
                                  PersonId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS AttachmentTruck (
                                  AttachmentId INTEGER,
                                  TruckId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS AttachmentWeapon (
                                  AttachmentId INTEGER,
                                  WeaponId TEXT)",
                                @"CREATE TABLE IF NOT EXISTS AttachmentPerson (
                                  AttachmentId INTEGER,
                                  PersonId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS UnitTree (
                                  UnitTreeId INTEGER,
                                  UnitId INTEGER,
                                  ParentId INTEGER)",
                                @"CREATE TABLE IF NOT EXISTS Manifests (
                                  ManifestId INTEGER,
                                  CreatedOn TEXT,
                                  CreatedBy INTEGER,
                                  EditedOn TEXT,
                                  EditedBy INTEGER,
                                  JSONBlob TEXT)"
                            };

                            // Run the queries
                            foreach (String table in tables) {
                                // Set the command text
                                comm.CommandText = table;

                                // Execute the command
                                comm.ExecuteNonQuery();
                            }
                        }

                        // Close the db
                        conn.Close();

                        // Set the created flag
                        isCreated = true;
                    }
                } catch (Exception) {
                    // Close the connection
                    conn.Close();
                }
            }
        }

        internal void syncData (List<string> syncData) {
            throw new NotImplementedException();
        }
    }
}
