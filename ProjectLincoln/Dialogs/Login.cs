using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ProjectLincoln.Dialogs {
    public partial class Login : Form {
        public Login () {
            InitializeComponent();
        }

        public string Username {
            get { return txtUsername.Text; }
        }
        public string Password {
            get { return txtPassword.Text; }
        }

        private void Login_FormClosing (object sender, FormClosingEventArgs e) {
            if (DialogResult == DialogResult.OK && checkForm()) {
                e.Cancel = true;
                MessageBox.Show("You must supply both the username and password.");
            }
        }

        private void txtUsername_Validating (object sender, CancelEventArgs e) {
            string errorMessage = "";
            bool error = false;

            // Ensure that there is a username
            if (txtUsername.Text.Length < 3) {
                // Set the error message and flag
                errorMessage = "Username is required.";
                error = true;
            }

            // Use the error flag
            if (error) {
                // Cancel the submitting event
                e.Cancel = true;

                // Select the textbox
                txtUsername.Focus();

                // Set the error
                errorProvider1.SetError(txtUsername, errorMessage);
            }
        }

        private void txtPassword_Validating (object sender, CancelEventArgs e) {
            string errorMessage = "";
            bool error = false;

            // Ensure that there is a username
            if (txtPassword.Text.Length < 8) {
                // Set the error message and flag
                errorMessage = "Password is required and it must be more than 8 characters.";
                error = true;
            }

            // Use the error flag
            if (error) {
                // Cancel the submitting event
                e.Cancel = true;

                // Select the textbox
                txtPassword.Focus();

                // Set the error
                errorProvider1.SetError(txtPassword, errorMessage);
            }
        }

        private void txtUsername_Validated (object sender, EventArgs e) {
            // Clear the error
            errorProvider1.SetError(txtUsername, "");
        }

        private void txtPassword_Validated (object sender, EventArgs e) {
            // Clear the error
            errorProvider1.SetError(txtPassword, "");
        }

        private void txtUsername_TextChanged (object sender, EventArgs e) {
            checkForm();
        }

        private void txtPassword_TextChanged (object sender, EventArgs e) {
            checkForm();
        }

        private bool checkForm () {
            // Check if there is a username and password
            if (txtUsername.Text.Length < 3 || txtPassword.Text.Length < 8) {
                // Disable the ok button
                btnLogin.Enabled = false;

                // Return if there is an error
                return true;
            } else {
                // Enable the ok button
                btnLogin.Enabled = true;

                // Return if there is an error
                return false;
            }
        }
    }
}
