using System.Collections.Generic;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Properties;
using System;
using System.Windows.Forms;

namespace ProjectLincoln.Services {
    class SyncService : ServiceAbstract {

        public static List<string> getInitialSyncData () {

            // Initialize the insertStatements
            List<string> insertStatements = new List<string>();

            // Initialize the local lists
            List<int> 
                attachmentIds = new List<int>(),
                personnelIds = new List<int>(),
                syncUnits = new List<int>() { Settings.Default.UnitId },
                truckIds = new List<int>(),
                weaponIds = new List<int>();

            #region Unit Levels
            // Add the insert statements to the insertStatements for the unit levels
            insertStatements.AddRange(UnitLevelService.retrieveUnitLevels().getInserts(DatabaseType.SQLite));
            #endregion
            #region Billets
            // Add the insert statements to the insertStatements for the billets
            insertStatements.AddRange(BilletService.retrieveBillets().getInserts(DatabaseType.SQLite));
            #endregion
            #region Ranks
            // Add the insert statements to the insertStatements for the ranks
            insertStatements.AddRange(RankService.retrieveRanks().getInserts(DatabaseType.SQLite));
            #endregion
            #region Services
            // Add the insert statements to the insertStatements for the services
            insertStatements.AddRange(ServicesService.retrieveServices().getInserts(DatabaseType.SQLite));
            #endregion
            #region TAMCNs
            // Add the insert statements to the insertStatements for the tamcns
            insertStatements.AddRange(TamncService.retrieveTamcns().getInserts(DatabaseType.SQLite));
            #endregion
            #region AttachmentTypes
            // Add the insert statements to the insertStatements for the attachment types
            insertStatements.AddRange(AttachmentTypeService.retrieveTypes().getInserts(DatabaseType.SQLite));
            #endregion
            #region WeaponTypes
            // Add the insert statements to the insertStatements for the weapon types
            insertStatements.AddRange(WeaponTypeService.retrieveTypes().getInserts(DatabaseType.SQLite));
            #endregion
            #region Units and the tree
            // Initialize the service
            UnitTreeService utService = null;

            #region Parents
            // Get the parents from the unit tree and their insert statements
            utService = UnitTreeService.getUnitTree(Settings.Default.UnitId, TreeTraversalDirection.Parents);
            insertStatements.AddRange(utService.getCombinedLists(true, DatabaseType.SQLite));
            #endregion

            // Get the top-most unit's insert statement
            insertStatements.Add(UnitService.getUnit(utService.getLastParent()).ToInsert(DatabaseType.SQLite));

            #region Children
            // Get the parents from the unit tree and their insert statements
            utService = UnitTreeService.getUnitTree(Settings.Default.UnitId, TreeTraversalDirection.Children);
            insertStatements.AddRange(utService.getCombinedLists(true, DatabaseType.SQLite));
            #endregion

            // Add the list of child units to the sync list
            syncUnits.AddRange(utService.getUnitIds());
            #endregion
            #region Trucks
            // Initialize the service
            TruckService truckService = TruckService.retrieveTrucks(syncUnits);

            // Get the list of truck ids
            truckService.getTrucks().ForEach(delegate (Truck truck) {
                truck.TruckId.IfNotNull(id => truckIds.Add(id));
            });

            // Add the insert statements to the insertStatements for the services
            insertStatements.AddRange(truckService.getInserts(DatabaseType.SQLite));
            #endregion
            #region Trailers 
            // Add the insert statements to the insertStatements for the trailers
            insertStatements.AddRange(TrailerService.retrieveTrailers().getInserts(DatabaseType.SQLite));
            #endregion
            #region Personnel
            // Initialize the service
            PersonService personService = PersonService.retrievePersons(syncUnits);

            // Get the list of person ids
            personService.getPersons().ForEach(delegate (Person person) {
                person.PersonId.IfNotNull(id => personnelIds.Add(id));
            });

            // Add the insert statements to the insertStatements for the services
            insertStatements.AddRange(personService.getInserts(DatabaseType.SQLite));
            #endregion
            #region Trucks' Weapons
            // Initialize the weapon trucks service and retrieve the data
            WeaponTruckService wtService = WeaponTruckService.retrieveWeapons(truckIds);

            // Get the weapon's ids
            wtService.getWeapons().ForEach(delegate (Weapon weapon) {
                weapon.WeaponId.IfNotNull(id => weaponIds.Add(id));
            });

            // Add the insert statements to the insertStatements for the trucks' weapons
            insertStatements.AddRange(wtService.getInserts(DatabaseType.SQLite));
            #endregion
            #region Personnels' Weapon
            // Initialize the weapon trucks service and retrieve the data
            WeaponPersonService pService = WeaponPersonService.retrieveWeapons(personnelIds);

            // Get the weapon's ids and if not duplicate, add to both lists
            pService.getWeapons().ForEach(delegate (Weapon weapon) {
                weapon.WeaponId.IfNotNull(weaponId => {
                    weaponIds.IfNotContains(weaponId, id => {
                        attachmentIds.Add(id);
                        insertStatements.Add(weapon.ToInsert());
                    });
                });
            });
            #endregion
            #region Weapons' attachments
            // Initialize the attachment weapon service and retrieve the data
            AttachmentWeaponService awService = AttachmentWeaponService.retrieveAttachments(weaponIds);

            // Add the attachments to the id list
            awService.getAttachments().ForEach(delegate (Attachment attachment) {
                attachment.AttachmentId.IfNotNull(id => attachmentIds.Add(id));
            });

            // Add the insert statements to the insertStatements for the personnels' weapons
            insertStatements.AddRange(awService.getInserts(DatabaseType.SQLite));
            #endregion
            #region Personnels' attachments
            // Initialize the attachment person service and retrieve the data
            AttachmentPersonService apService = AttachmentPersonService.retrieveAttachments(personnelIds);

            apService.getAttachments().ForEach(delegate (Attachment attachment) {
                attachment.AttachmentId.IfNotNull(attachmentId => {
                    attachmentIds.IfNotContains(attachmentId, id => {
                        attachmentIds.Add(id);
                        insertStatements.Add(attachment.ToInsert());
                    });
                });
            });
            #endregion
            insertStatements.Clear();
            #region Personnels' attachments
            // Initialize the attachment person service and retrieve the data
            AttachmentTruckService atService = AttachmentTruckService.retrieveAttachments(truckIds);

            atService.getAttachments().ForEach(delegate (Attachment attachment) {
                attachment.AttachmentId.IfNotNull(attachmentId => {
                    attachmentIds.IfNotContains(attachmentId, id => {
                        attachmentIds.Add(id);
                        insertStatements.Add(attachment.ToInsert());
                    });
                });
            });
            #endregion
            //#region Attachments
            //// Initialize the attachment weapon service and retrieve the data
            ////AttachmentService aService = AttachmentService.retrieveAttachments(weaponTruckIds);

            //// Add the insert statements to the insertStatements for the trucks' weapons
            ////insertStatements.AddRange(wtService.getInserts(DatabaseType.SQLite));
            //#endregion

            // Return the array of insert statements
            return insertStatements;
        }
    }
}
