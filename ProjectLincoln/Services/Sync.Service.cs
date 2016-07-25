﻿using System.Collections.Generic;

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
            splash.updateLoadingMessage(string.Format("Getting the appropriate {0} data", region));
        }

        /// <summary>
        /// Queries the cloud db for all of the needed data to populate the local db
        /// </summary>
        /// <param name="splash">The local reference to the splash scree</param>
        /// <returns>A list of string insert statements</returns>
        public static List<string> getInitialSyncData (Splash splash) {
            // Initialize this class
            SyncService service = new SyncService(splash);

            // Initialize the insertStatements
            List<string> insertStatements = new List<string>();

            // Initialize the local lists
            List<int> 
                attachmentIds = new List<int>(),
                personnelIds = new List<int>(),
                syncUnits = new List<int>() { Settings.Default.UnitId },
                truckIds = new List<int>(),
                weaponIds = new List<int>();
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
                unitTreeInserts = new List<string>();

            #region TAMCNs
            // Update the loading message
            service.updateLoadingMessage("TAMCNs");

            // Add the insert statements to the insertStatements for the tamcns
            tamcnsInserts = TamncService.retrieveTamcns().getInserts(DatabaseType.SQLite);
            #endregion
            #region Unit Levels
            // Update the loading message
            service.updateLoadingMessage("Unit Levels");

            // Add the insert statements to the insertStatements for the unit levels
            unitLevelInserts = UnitLevelService.retrieveUnitLevels().getInserts(DatabaseType.SQLite);
            #endregion
            #region Billets
            // Update the loading message
            service.updateLoadingMessage("Billets");

            // Add the insert statements to the insertStatements for the billets
            billetInserts = BilletService.retrieveBillets().getInserts(DatabaseType.SQLite);
            #endregion
            #region Ranks
            // Update the loading message
            service.updateLoadingMessage("Ranks");

            // Add the insert statements to the insertStatements for the ranks
            ranksInserts = RankService.retrieveRanks().getInserts(DatabaseType.SQLite);
            #endregion
            #region Services
            // Update the loading message
            service.updateLoadingMessage("Services");

            // Add the insert statements to the insertStatements for the services
            servicesInserts = ServicesService.retrieveServices().getInserts(DatabaseType.SQLite);
            #endregion
            #region AttachmentTypes
            // Update the loading message
            service.updateLoadingMessage("Attachment Types");

            // Add the insert statements to the insertStatements for the attachment types
            attachmentTypeInserts = AttachmentTypeService.retrieveTypes().getInserts(DatabaseType.SQLite);
            #endregion
            #region WeaponTypes
            // Update the loading message
            service.updateLoadingMessage("Weapon Types");

            // Add the insert statements to the insertStatements for the weapon types
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
            insertStatements.AddRange(utService.getCombinedLists(true, DatabaseType.SQLite));
            #endregion

            // Get the top-most unit's insert statement
            insertStatements.Add(UnitService.getUnit(utService.getLastParent()).ToInsert(DatabaseType.SQLite));

            #region Children
            // Update the loading message
            service.updateLoadingMessage("Children Units");

            // Get the parents from the unit tree and their insert statements
            utService = UnitTreeService.getUnitTree(Settings.Default.UnitId, TreeTraversalDirection.Children);
            insertStatements.AddRange(utService.getCombinedLists(true, DatabaseType.SQLite));
            #endregion

            // Add the list of child units to the sync list
            syncUnits.AddRange(utService.getUnitIds());
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

            // Add the insert statements to the insertStatements for the services
            insertStatements.AddRange(truckService.getInserts(DatabaseType.SQLite));
            #endregion
            #region Trailers 
            // Update the loading message
            service.updateLoadingMessage("Trailers");

            // Add the insert statements to the insertStatements for the trailers
            insertStatements.AddRange(TrailerService.retrieveTrailers().getInserts(DatabaseType.SQLite));
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

            // Add the insert statements to the insertStatements for the services
            insertStatements.AddRange(personService.getInserts(DatabaseType.SQLite));
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

            // Add the insert statements to the insertStatements for the trucks' weapons
            insertStatements.AddRange(wtService.getInserts(DatabaseType.SQLite));
            #endregion
            #region Personnels' Weapons
            // Update the loading message
            service.updateLoadingMessage("Personnels' Weapons");

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
            // Update the loading message
            service.updateLoadingMessage("Weapons' attachments");

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
            // Update the loading message
            service.updateLoadingMessage("Personnels' attachments");

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
            #region Trucks' attachments
            // Update the loading message
            service.updateLoadingMessage("Trucks' attachments");

            // Initialize the attachment truck service and retrieve the data
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
