using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class UnitTreeService : ServiceAbstract {

        private List<Unit> units = null;
        private List<UnitTree> unitTrees = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTreeService () {
            units = new List<Unit>();
            unitTrees = new List<UnitTree>();
        }

        /// <summary>
        /// Gets the id of the last parent in the parents list
        /// </summary>
        /// <returns>The last parent's id</returns>
        public int getLastParent () {
            return unitTrees[unitTrees.Count - 1].ParentId;
        }

        /// <summary>
        /// Gets the ids of all units that need to be synced
        /// </summary>
        /// <returns>The list of unit ids</returns>
        public List<int> getUnitIds () {
            // Initialize the output
            List<int> unitIds = new List<int>();

            // Loop through the units and extract the ids
            foreach (Unit unit in units) {
                unitIds.Add((int) unit.UnitId);
            }

            // Return the ids
            return unitIds;
        }

        /// <summary>
        /// Gets the unit tree for the given unitId
        /// </summary>
        /// <param name="unitId">The id number of the requested unit</param>
        /// <param name="dir">The direction to traverse the tree</param>
        /// <returns>A new instance of this class</returns>
        public static UnitTreeService getUnitTree (int unitId, TreeTraversalDirection dir) {
            // Create a new instance of this class
            UnitTreeService utService = new UnitTreeService();

            // Prepare the db
            MySQL db = null;

            try {
                #region Try
                // Create the select statement
                string tmpTable = "";
                switch (dir) {
                    case TreeTraversalDirection.Parents:
                        // Create the temporary table's querystring
                        tmpTable = Select.create("UnitTree", "UT", new List<string>() {"ParentId", "UnitTreeId"})
                        .join("Unit", "U", "UT.UnitId = U.UnitId", JoinType.Left, new List<string>() {"*"})
                        .order("ParentId", OrderType.DESC)
                        .ToInnerString();

                        // Create the querystring
                        select = Select.create("(" + tmpTable + ")", "t", new List<string>() { "UnitId", "Name", "Abbreviation", "ServiceId", "UnitTreeId", "UnitLevelId" })
                                .join("(SELECT @parent := ?UnitId)", "tmp", null, JoinType.Join)
                                .where("t.UnitId = @parent")
                                .columns(new List<string>() { "@parent := t.ParentId ParentId" });
                        break;
                    case TreeTraversalDirection.Children:
                        // Create the temporary table's querystring
                        tmpTable = Select.create("UnitTree", "UT", new List<string>() {"ParentId", "UnitTreeId"})
                                .join("Unit", "U", "UT.UnitId = U.UnitId", JoinType.Left, new List<string>() {"*"})
                                .order("UT.ParentId")
                                .order("U.UnitId")
                                .ToInnerString();

                        // Create the querystring
                        select = Select.create("(" + tmpTable + ")", "t", new List<string>() { "UnitId", "Name", "Abbreviation", "ServiceId", "UnitTreeId", "ParentId", "UnitLevelId" })
                                .join("(SELECT @parent := ?UnitId)", "tmp", null, JoinType.Inner)
                                .where("find_in_set(ParentId, @parent) > 0")
                                .where("@parent := concat(@parent, ',', UnitId)");
                        break;
                }

                // Open the connection
                db = MySQL.openConnection(select, new string[] {
                    "?UnitId",  unitId.ToString()});

                // Read the rows
                MySqlDataReader reader = db.reader;
                while (db.reader.Read()) {
                    // Add the unit to the array
                    utService.units.Add(Unit.getFromDb(reader));

                    // Add the unitTree to the array
                    utService.unitTrees.Add(UnitTree.getFromDb(reader));
                }

                // Close the connection and reader
                db.closeDB();
                #endregion
            } catch (Exception ex) {
                #region catch
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

            // Return the instance of this class
            return utService;
        }

        /// <summary>
        /// Returns a combined string list of the two arrays
        /// </summary>
        /// <param name="isInsert">True for insert statements, false for formatted strings</param>
        /// <param name="dbType">The type of database this will be inserted into</param>
        /// <returns>A string list of the combined lists</returns>
        public List<string> getCombinedLists (bool isInsert, DatabaseType dbType = DatabaseType.MySql) {
            // Initialize the output
            List<string> output = new List<string>();

            // Loop through the units array
            foreach (Unit unit in units) {
                output.Add((isInsert) ? unit.ToInsert(dbType) : unit.ToString());
            }

            // Loop through the unitTrees array
            foreach (UnitTree unitTree in unitTrees) {
                output.Add((isInsert) ? unitTree.ToInsert(dbType) : unitTree.ToString());
            }

            // Return the combined array
            return output;
        }
    }

    public enum TreeTraversalDirection {
        Parents, Children
    }
}
