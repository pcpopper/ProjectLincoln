using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.NetworkInformation;

using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Properties;
using ProjectLincoln.Services;

namespace ProjectLincoln {
    public partial class Splash : Form {

        // Flags that are used in the steps
        private bool hasConnection = false,
            loggedIn = false,
            nextFlag = true,
            initial = true,
            updatesAvailable = false;

        // List of insert statements
        List<string> syncData = new List<string>();

        private int timeout = 60; // Stops after x seconds
        private int loginAttemptsAvailable = 3; // The number of allowed attempts before closing out

        // Integers used while starting up
        private int timeLeft = 0, step = -1, loginAttempts = 0;

        private SQLite localDb = new SQLite();
        private MySQL cloudDb = new MySQL();
        private List<string> loadingMessages = new List<string> () {
                    "Testing internet connection.", // Step 0
                    "Logging in.", // Step 1
                    "Initializing local data.", // Step 2
                    "Downloading data for the first time.", // Step 3
                    "Installing the data locally", // Step 4
                    "Checking for updates.", // Step 5
                    "Updating data.", // Step 6
                    "Building Equipment.", // Step 7
                    "Gathering People.", // Step 8
                    "Loading Forms" // Step 9
                };
        private Dialogs.Login loginDialog = null;

        public Splash () {
            InitializeComponent();
        }

        public void updateLoadingMessage (string message) {
            lblSpecifics.Text = message;
        }

        private void Splash_Load (object sender, EventArgs e) {
            // Double the timeout time to convert it to seconds
            timeout *= 2;
            timeLeft = timeout;

            //listBox1.Visible = false;
            //Select select = Helpers.DB.Select.create("Unit")
            //        .where("UnitId = ?UnitId");
            //textBox1.Text = select.ToString();

            // Wait for the UI to update
            this.Invoke((MethodInvoker) delegate {
                tmr1.Start(); // Start the timer
            });
        }

        private void tmr1_Tick (object sender, EventArgs e) {
            timeLeft--; // Decrement the timeLeft

            // Stop the timer if it reaches 0
            if (timeLeft <= 0) {
                // Stop the timer
                tmr1.Stop();

                // Exit the application
                Application.Exit();
            } else {
                this.Invoke((MethodInvoker) delegate {
                #region Processing the next steps
                if (nextFlag) {
                        // Reset the time left
                        timeLeft = timeout;

                        // Increment the step
                        step++;

                        // Update the loading message
                        lblLoading.Text = loadingMessages[step];

                        // Reset the next flag
                        nextFlag = false;
                    }
                #endregion

                // Check what step it is
                switch (step) {
                        #region Testing internet connection
                        case 0:
                            try {
                                // Set the connection variables
                                updateLoadingMessage("Setting up the ping");
                                String host = "google.com";
                                byte[] buffer = new byte[32];
                                int timeout = 1000;

                                // Ping the site
                                updateLoadingMessage("Ping. Pong.");
                                Ping myPing = new Ping();
                                PingOptions pingOptions = new PingOptions();
                                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);

                                // Set isConnected to the connection status
                                updateLoadingMessage("Checking ping reply");
                                hasConnection = (reply.Status == IPStatus.Success);
                            } catch (Exception) {
                                // Check if there is a login stored
                                if (Settings.Default.Username.Equals("")) {
                                    // Stop the timer
                                    tmr1.Stop();

                                    // There is not. Display error message and quit
                                    MessageBox.Show("There needs to be an internet connection in order to initialize this system.\n\nPlease connect before reopening this program.", "No internet connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    // Quit the application
                                    Application.Exit();
                                }

                                // Set isConnected to false as there is no connection
                                hasConnection = false;
                            }

                            // Bump to skip over the login
                            if (Settings.Default.DevMode) {
                                step++;
                                Settings.Default.Username = "dev";
                                Settings.Default.UnitId = 7;
                                Settings.Default.IsDevAdmin = true;
                                Settings.Default.IsUnitAdmin = true;
                                Settings.Default.IsUserAdmin = true;
                            }

                            // Set the next flag
                            nextFlag = true;
                            break;
                        #endregion
                        #region Logging in
                        case 1:
                            // Create a canceled flag
                            bool canceled = false;

                            // Create the login form
                            updateLoadingMessage("Creating the login dialog");
                            loginDialog = new Dialogs.Login();

                            // Stop the timer to insure that the splash screen doesn't time out while waiting
                            tmr1.Stop();

                            // Keep showing the dialog until one part fails
                            while (!loggedIn && loginAttempts < loginAttemptsAvailable) {
                                // Show the login
                                updateLoadingMessage("Showing the login dialog");
                                if (loginDialog.ShowDialog() == DialogResult.OK) {
                                    // Check for a username in the application's settings
                                    if (!Settings.Default.Username.Equals("")) {
                                        #region Local
                                        // Check the username and password against the stored values
                                        updateLoadingMessage("Checking username and password locally");
                                        if (loginDialog.Username.Equals(Settings.Default.Username) && 
                                            loginDialog.Password.Equals(Settings.Default.Password)) {
                                            // Set the logged in flag
                                            loggedIn = true;

                                            // Jump to step 4, skipping the initial setup steps
                                            step = 3;
                                        } else {
                                            // Show the error
                                            MessageBox.Show("The credentials that you provided was not found.\n\nPlease try again.", "Login Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                                            // Increment the number of login attempts
                                            loginAttempts++;
                                        }
                                        #endregion
                                    } else {
                                        #region Cloud
                                        updateLoadingMessage("Checking username and password remotely");
                                        // Set that this is the initial load
                                        initial = true;

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
                            loginDialog.Dispose();

                            // Check if the loading was canceled
                            if (canceled) {
                                // Quit the application
                                Application.Exit();
                            }

                            // Check if logged in
                            if (loggedIn) {
                                // Reset the timer and start it again
                                timeLeft = timeout;
                                tmr1.Start();

                                // Set the next flag
                                nextFlag = true;
                            } else {
                                // Display error message and quit
                                MessageBox.Show("You need to be logged in to use this application.\n\nPlease try again with the proper credentials", "Incorrect Authentication!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                // Quit the application
                                Application.Exit();
                            }
                            break;
                        #endregion
                        #region Initializing Local DB
                        case 2:
                            // Pass the reference to this form
                            localDb.setSplash(this);

                            // Initialize the local db
                            localDb.createDatabase();

                            // Set the next flag
                            nextFlag = true;
                            break;
                        #endregion
                        #region Downloading data from the cloud
                        case 3:
                            // Check if there is an internet connection
                            if (hasConnection) {
                                // Wait for the local db to become ready
                                if (localDb.isCreated && initial) {
                                    // Get the data from the cloud
                                    syncData = SyncService.getInitialSyncData(this);

                                    // Check if there is any sync data
                                    if (syncData.Count == 0) {
                                        // Stop the time to allow for the message box
                                        tmr1.Stop();

                                        // Show the error
                                        DialogResult msgBox = MessageBox.Show("Error Getting Data",
                                            "There was an error while getting data.",
                                            MessageBoxButtons.RetryCancel,
                                            MessageBoxIcon.Error);

                                        if (msgBox == DialogResult.Retry) {
                                            // Reset the time left
                                            timeLeft = timeout;

                                            // Restart the timer
                                            tmr1.Start();

                                            // Retry to get the data from the cloud
                                            syncData = SyncService.getInitialSyncData(this);

                                            // Check if there is any sync data
                                            if (syncData.Count == 0) {
                                                // Stop the time to allow for the message box
                                                tmr1.Stop();

                                                // Show the error again
                                                msgBox = MessageBox.Show("Error Getting Data",
                                                    "There was an error while getting data.",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);

                                                if (msgBox == DialogResult.OK) {
                                                    Application.Exit();
                                                }
                                            }
                                        } else if (msgBox == DialogResult.Cancel) {
                                            Application.Exit();
                                        }
                                    }
                                }
                            }

                            // Set the next flag
                            nextFlag = true;
                            break;
                        #endregion
                        #region Inserting data locally
                        case 4:
                            // Insert the data into the local db
                            localDb.syncData(syncData);

                            // Skip the next step
                            //step++;

                            // Set the next flag
                            //nextFlag = true;
                            break;
                        #endregion
                        #region Check cloud db for new data
                        case 5:
                            // Check if there is an internet connection
                            if (hasConnection) {
                                // Wait for the local db to become ready
                                if (!initial) {
                                    // Check the cloud db for updates
                                    updatesAvailable = cloudDb.checkForUpdates();
                                }
                            }

                            // Set the next flag
                            nextFlag = true;
                            break;
                        #endregion
                        case 6:
                            break;
                        case 7:
                            break;
                        case 8:
                            break;
                        case 9:
                            break;
                        case 10:
                            // Load the main form
                            FormMain main = new FormMain();
                            main.Show();

                            // Hide the splash screen
                            this.Hide();
                            break;
                    }
                });
            }
        }
    }
}
