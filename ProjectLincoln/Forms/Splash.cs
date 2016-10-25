using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.NetworkInformation;

using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;
using ProjectLincoln.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectLincoln {
    public partial class Splash : Form {

        private readonly SynchronizationContext synchronizationContext;

        private FormMain main;

        public Splash (FormMain main) {
            this.main = main;

            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        public void updateLoadingMessage (SplashElement el, string message) {
            synchronizationContext.Post(new SendOrPostCallback(o => {
                switch (el) {
                    case SplashElement.Label:
                        lblSpecifics.Text = message;
                        break;
                    case SplashElement.Text:
                        lblLoading.Text = message;
                        break;
                }
            }), message);
        }

        private void Splash_Load (object sender, EventArgs e) {
            asyncLoadSplash();
        }

        private async void asyncLoadSplash () {
            SQLite localDb = new SQLite();
            MySQL cloudDb = new MySQL();

            List<string> syncData = new List<string>(); // List of insert statements

            Dialogs.Login loginDialog = null; // The login dialog

            // Flags that are used in the loading
            bool hasConnection = false,
                initial = (Settings.Default.Username.Equals("")),
                loggedIn = false,
                updatesAvailable = false;
            int attemptsAvailable = 3, // The number of allowed attempts before closing out
                loginAttempts = 0,
                captureSyncAttempts = 0;

            if (Settings.Default.DevMode) {
                Settings.Default.Username = "dev";
                Settings.Default.UnitId = 7;
                Settings.Default.IsDevAdmin = true;
                Settings.Default.IsUnitAdmin = true;
                Settings.Default.IsUserAdmin = true;
            }

            await Task.Run(() => {
                #region Testing the Internet connection
                updateLoadingMessage(SplashElement.Text, "Testing the Internet connection.");
                try {
                    // Set the connection variables
                    updateLoadingMessage(SplashElement.Label, "Setting up the ping.");
                    String host = "google.com";
                    byte[] buffer = new byte[32];
                    int timeout = 1000;

                    // Ping the site
                    updateLoadingMessage(SplashElement.Label, "Ping. Pong.");
                    Ping myPing = new Ping();
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);

                    // Set isConnected to the connection status
                    updateLoadingMessage(SplashElement.Label, "Checking ping reply.");
                    hasConnection = (reply.Status == IPStatus.Success);
                } catch (PingException) {
                    // Do nothing as this is an error that is already accounted for by initializing 'hasConnection' to false
                }

                // Show an error if there is no connection and this is the first load
                if (initial && !hasConnection) {
                    // There is not. Display error message and quit
                    MessageBox.Show("There needs to be an Internet connection in order to initialize this system.\n\nPlease connect before reopening this program.", "No internet connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Quit the application
                    Application.Exit();
                }
                #endregion
                #region Logging in
                if (!Settings.Default.DevMode) {
                    updateLoadingMessage(SplashElement.Text, "Logging in.");
                    // Create a canceled flag
                    bool canceled = false;

                    // Create the login form
                    updateLoadingMessage(SplashElement.Label, "Creating the login dialog.");
                    loginDialog = new Dialogs.Login();

                    // Keep showing the dialog until one part fails
                    while (!loggedIn && loginAttempts < attemptsAvailable) {
                        // Show the login
                        updateLoadingMessage(SplashElement.Text, "Showing the login dialog.");
                        if (loginDialog.ShowDialog() == DialogResult.OK) {
                            // Check for a username in the application's settings
                            if (!initial) {
                                #region Local
                                // Check the username and password against the stored values
                                updateLoadingMessage(SplashElement.Text, "Checking username and password locally.");
                                if (loginDialog.Username.Equals(Settings.Default.Username) &&
                                    loginDialog.Password.Equals(Settings.Default.Password)) {
                                    // Set the logged in flag
                                    loggedIn = true;
                                } else {
                                    // Show the error
                                    MessageBox.Show("The credentials that you provided was not found.\n\nPlease try again.", "Login Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                                    // Increment the number of login attempts
                                    loginAttempts++;
                                }
                                #endregion
                            } else {
                                #region Cloud
                                updateLoadingMessage(SplashElement.Text, "Checking username and password remotely");
                                // Call the login function in the SQL class
                                if (UserService.checkCredentials(loginDialog.Username, loginDialog.Password)) {
                                    // Set the logged in flag
                                    loggedIn = true;
                                } else {
                                    // Show the error
                                    MessageBox.Show("The credentials that you provided was not found.\n\nPlease try again.", "Login Error!", MessageBoxButtons.OK);

                                    // Increment the number of login attempts
                                    loginAttempts++;
                                }
                                #endregion
                            }
                        } else { // The cancel or close button was pressed
                                 // Set the canceled flag
                            canceled = true;

                            // Exit the loop
                            break;
                        }
                    }

                    // Close the login dialog
                    updateLoadingMessage(SplashElement.Text, "Closing the login dialog.");
                    loginDialog.Dispose();

                    // Check if the loading was canceled
                    if (canceled) {
                        // Quit the application
                        Application.Exit();
                    }

                    // Check if logged in
                    if (!loggedIn) {
                        // Display error message and quit
                        MessageBox.Show("You need to be logged in to use this application.\n\nPlease try again with the proper credentials", "Incorrect Authentication!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Quit the application
                        Application.Exit();
                    }
                }
                #endregion
                #region Initializing Local DB
                // Check if this is the initial load
                if (initial) {
                    updateLoadingMessage(SplashElement.Text, "Initializing local data.");
                    // Pass the reference to this form
                    localDb.setSplash(this);

                    // Initialize the local db
                    localDb.createDatabase();
                }
                #endregion
                #region Downloading data from the cloud
                // Check if there is an Internet connection and this is the initial load
                if (hasConnection && initial) {
                    updateLoadingMessage(SplashElement.Text, "Downloading data for the first time.");

                    // Keep showing the dialog until one part fails
                    while (syncData.Count == 0 && captureSyncAttempts < attemptsAvailable) {
                        // Get the data from the cloud
                        syncData = SyncService.getInitialSyncData(this);

                        // Check if there is any sync data
                        if (syncData.Count == 0) {
                            // Bump up the attempts
                            captureSyncAttempts++;

                            // Show the error
                            if (captureSyncAttempts == attemptsAvailable) {
                                DialogResult msgBox = MessageBox.Show("Error Getting Data",
                                            "There was an error while getting data.",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);

                                // Exit the application as it has reached the max errors
                                Application.Exit();
                            } else {
                                DialogResult msgBox = MessageBox.Show("Error Getting Data",
                                            "There was an error while getting data.",
                                            MessageBoxButtons.RetryCancel,
                                            MessageBoxIcon.Error);

                                // Exit the application as the user has clicked cancel
                                if (msgBox == DialogResult.Cancel) {
                                    Application.Exit();
                                }
                            }
                        }
                    }
                }
                #endregion
                #region Inserting data locally
                if (initial) {
                    updateLoadingMessage(SplashElement.Text, "Installing the data locally");

                    // Insert the data into the local db
                    localDb.syncData(syncData);

                    // Update the setting for the last sync time
                    Settings.Default.LastSync = DateTime.Now;
                }
                #endregion
                #region Check cloud db for new data
                // Check if there is an Internet connection
                if (hasConnection && !initial) {
                    updateLoadingMessage(SplashElement.Text, "Checking for updates.");

                    // Check the cloud db for updates
                    updatesAvailable = UnitService.checkForUpdates(this);
                }
                #endregion
                #region Pre-loading forms
                updateLoadingMessage(SplashElement.Text, "Loading Forms");
                #region Building equipment elements
                updateLoadingMessage(SplashElement.Text, "Building Equipment.");
                #endregion
                #region Building people elements
                updateLoadingMessage(SplashElement.Text, "Gathering People.");
                #endregion
                #endregion
            });

            // Show the main form
            main.Show();

            // Close the splash screen
            this.Close();
        }
    }

    public enum SplashElement {
        Label, Text
    }
}
