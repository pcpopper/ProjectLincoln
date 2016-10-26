using System.Collections.Generic;

using ProjectLincoln.Entity;
using ProjectLincoln.Enums;
using ProjectLincoln.Properties;

namespace ProjectLincoln.Services {
    class SyncService : ServiceAbstract {

        private Splash splash = null; // Local reference to the splash screen

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="splash">The local reference to the splash scree</param>
        SyncService (Splash splash) {
            this.splash = splash;
        }

        /// <summary>
        /// Creates and updates the loading message
        /// </summary>
        /// <param name="region">The region that is calling this update</param>
        private void updateLoadingMessage (string region) {
            // Update the loading message
            splash.updateLoadingMessage(SplashElement.Label, string.Format("Getting the appropriate {0} data", region));
        }

        /// <summary>
        /// Queries the cloud db for all of the needed data to populate the local db
        /// </summary>
        /// <param name="splash">The local reference to the splash scree</param>
        /// <returns>A list of string insert statements</returns>
        public static List<string> getInitialSyncData (Splash splash) {
            // Initialize this class
            SyncService service = new SyncService(splash);

            // Initialize the local lists
            #region int Lists
            List<int> 
                attachmentIds = new List<int>(),
                personnelIds = new List<int>(),
                syncUnits = new List<int>() { Settings.Default.UnitId },
                truckIds = new List<int>(),
                weaponIds = new List<int>();
            #endregion
            #region string Lists
            List<string>
                tamcnsInserts = new List<string>(),
                weaponTypeInserts = new List<string>(),
                weaponInserts = new List<string>(),
                attachmentTypeInserts = new List<string>(),
                attachmentInserts = new List<string>(),
                servicesInserts = new List<string>(),
                unitLevelInserts = new List<string>(),
                unitInserts = new List<string>(),
                trailerInserts = new List<string>(),
                truckInserts = new List<string>(),
                weaponTruckInserts = new List<string>(),
                ranksInserts = new List<string>(),
                billetInserts = new List<string>(),
                personInserts = new List<string>(),
                weaponPersonInserts = new List<string>(),
                attachmentTruckInserts = new List<string>(),
                attachmentWeaponInserts = new List<string>(),
                attachmentPersonInserts = new List<string>(),
                unitTreeInserts = new List<string>(),
                insertStatements = new List<string>();
            #endregion

            #region TAMCNs
            // Update the loading message
            service.updateLoadingMessage("TAMCNs");

            // Get the insert statements
            tamcnsInserts = TamcnService.retrieveTamcns().getInserts(DatabaseType.SQLite);
            #endregion
            #region Unit Levels
            // Update the loading message
            service.updateLoadingMessage("Unit Levels");

            // Get the insert statements
            unitLevelInserts = UnitLevelService.retrieveUnitLevels().getInserts(DatabaseType.SQLite);
            #endregion
            #region Billets
            // Update the loading message
            service.updateLoadingMessage("Billets");

            // Get the insert statements
            billetInserts = BilletService.retrieveBillets().getInserts(DatabaseType.SQLite);
            #endregion
            #region Ranks
            // Update the loading message
            service.updateLoadingMessage("Ranks");

            // Get the insert statements
            ranksInserts = RankService.retrieveRanks().getInserts(DatabaseType.SQLite);
            #endregion
            #region Services
            // Update the loading message
            service.updateLoadingMessage("Services");

            // Get the insert statements
            servicesInserts = ServicesService.retrieveServices().getInserts(DatabaseType.SQLite);
            #endregion
            #region AttachmentTypes
            // Update the loading message
            service.updateLoadingMessage("Attachment Types");

            // Get the insert statements
            attachmentTypeInserts = AttachmentTypeService.retrieveTypes().getInserts(DatabaseType.SQLite);
            #endregion
            #region WeaponTypes
            // Update the loading message
            service.updateLoadingMessage("Weapon Types");

            // Get the insert statements
            weaponInserts = WeaponTypeService.retrieveTypes().getInserts(DatabaseType.SQLite);
            #endregion
            #region Units and the tree
            // Initialize the service
            UnitTreeService utService = null;

            #region Parents
            // Update the loading message
            service.updateLoadingMessage("Parent Units");

            // Get the parents from the unit tree and their insert statements
            utService = UnitTreeService.getUnitTree(Settings.Default.UnitId, TreeTraversalDirection.Parents);
            unitInserts = utService.getCombinedLists(true, DatabaseType.SQLite);
            #endregion

            // Get the top-most unit's insert statement
            unitInserts.Add(UnitService.getUnit(utService.getLastParent()).ToInsert(DatabaseType.SQLite));

            #region Children
            // Update the loading message
            service.updateLoadingMessage("Children Units");

            // Get the parents from the unit tree and their insert statements
            utService = UnitTreeService.getUnitTree(Settings.Default.UnitId, TreeTraversalDirection.Children);
            unitInserts.AddRange(utService.getCombinedLists(true, DatabaseType.SQLite));

            // Add the list of child units to the sync list
            syncUnits.AddRange(utService.getUnitIds());
            #endregion
            #endregion
            #region Trucks
            // Update the loading message
            service.updateLoadingMessage("Trucks");

            // Initialize the service
            TruckService truckService = TruckService.retrieveTrucks(syncUnits);

            // Get the list of truck ids
            truckService.getTrucks().ForEach(delegate (Truck truck) {
                truck.TruckId.IfNotNull(id => truckIds.Add(id));
            });

            // Get the insert statements
            truckInserts = truckService.getInserts(DatabaseType.SQLite);
            #endregion
            #region Trailers 
            // Update the loading message
            service.updateLoadingMessage("Trailers");

            // Get the insert statements
            trailerInserts = TrailerService.retrieveTrailers().getInserts(DatabaseType.SQLite);
            #endregion
            #region Personnel
            // Update the loading message
            service.updateLoadingMessage("Personnel");

            // Initialize the service
            PersonService personService = PersonService.retrievePersons(syncUnits);

            // Get the list of person ids
            personService.getPersons().ForEach(delegate (Person person) {
                person.PersonId.IfNotNull(id => personnelIds.Add(id));
            });

            // Get the insert statements
            personInserts = personService.getInserts(DatabaseType.SQLite);
            #endregion
            #region Trucks' Weapons
            // Update the loading message
            service.updateLoadingMessage("Trucks' Weapons");

            // Initialize the weapon trucks service and retrieve the data
            WeaponTruckService wtService = WeaponTruckService.retrieveWeapons(truckIds);

            // Get the weapon's ids
            wtService.getWeapons().ForEach(delegate (Weapon weapon) {
                weapon.WeaponId.IfNotNull(id => weaponIds.Add(id));
            });

            // Get the insert statements
            weaponTruckInserts = wtService.getInserts(DatabaseType.SQLite);
            #endregion
            #region Personnels' Weapons
            // Update the loading message
            service.updateLoadingMessage("Personnels' Weapons");

            // Initialize the weapon trucks service and retrieve the data
            WeaponPersonService pService = WeaponPersonService.retrieveWeapons(personnelIds);

            // Get the weapon's ids
            pService.getWeapons().ForEach(delegate (Weapon weapon) {
                weapon.WeaponId.IfNotNull(weaponId => {
                    weaponIds.AddIfNotContains(weaponId);
                });
            });

            // Get the insert statements
            weaponPersonInserts = pService.getInserts();
            #endregion
            #region Weapons' attachments
            // Update the loading message
            service.updateLoadingMessage("Weapons' attachments");

            // Initialize the attachment weapon service and retrieve the data
            AttachmentWeaponService awService = AttachmentWeaponService.retrieveAttachments(weaponIds);

            // Add the attachments to the id list
            awService.getAttachments().ForEach(delegate (Attachment attachment) {
                attachment.AttachmentId.IfNotNull(id => attachmentIds.Add(id));
            });

            // Get the insert statements
            attachmentWeaponInserts = awService.getInserts(DatabaseType.SQLite);
            #endregion
            #region Personnels' attachments
            // Update the loading message
            service.updateLoadingMessage("Personnels' attachments");

            // Initialize the attachment person service and retrieve the data
            AttachmentPersonService apService = AttachmentPersonService.retrieveAttachments(personnelIds);

            apService.getAttachments().ForEach(delegate (Attachment attachment) {
                attachment.AttachmentId.IfNotNull(attachmentId => {
                    attachmentIds.AddIfNotContains(attachmentId);
                });
            });

            // Get the insert statements
            attachmentPersonInserts = apService.getInserts();
            #endregion
            #region Trucks' attachments
            // Update the loading message
            service.updateLoadingMessage("Trucks' attachments");

            // Initialize the attachment truck service and retrieve the data
            AttachmentTruckService atService = AttachmentTruckService.retrieveAttachments(truckIds);

            atService.getAttachments().ForEach(delegate (Attachment attachment) {
                attachment.AttachmentId.IfNotNull(attachmentId => {
                    attachmentIds.AddIfNotContains(attachmentId);
                });
            });

            // Get the insert statements
            attachmentTruckInserts = pService.getInserts();
            #endregion

            #region Concatenation
            // Add all of the statements to the output in the order that will not throw errors
            insertStatements = tamcnsInserts;
            insertStatements.AddRange(weaponTypeInserts);
            insertStatements.AddRange(weaponInserts);
            insertStatements.AddRange(attachmentTypeInserts);
            insertStatements.AddRange(attachmentInserts);
            insertStatements.AddRange(servicesInserts);
            insertStatements.AddRange(unitLevelInserts);
            insertStatements.AddRange(unitInserts);
            insertStatements.AddRange(trailerInserts);
            insertStatements.AddRange(truckInserts);
            insertStatements.AddRange(weaponTruckInserts);
            insertStatements.AddRange(ranksInserts);
            insertStatements.AddRange(billetInserts);
            insertStatements.AddRange(personInserts);
            insertStatements.AddRange(weaponPersonInserts);
            insertStatements.AddRange(attachmentTruckInserts);
            insertStatements.AddRange(attachmentWeaponInserts);
            insertStatements.AddRange(attachmentPersonInserts);
            insertStatements.AddRange(unitTreeInserts);
            #endregion

            // Return the array of insert statements
            return insertStatements;
        }
    }
}
