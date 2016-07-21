using System;
using System.Windows.Forms;

using ProjectLincoln.Helpers.DB;

namespace ProjectLincoln {
    public partial class FormMain : Form {
        public SQLite localDb = null;
        public MySQL cloudDb = null;

        public FormMain () {
            InitializeComponent();
        }

        private void Form1_Load (object sender, EventArgs e) {
            // Connect to the local db
            //addToList("Starting");
            //localDb = new SQLite();
            //MySQL cloudDb = new MySQL();
        }

        public void initializeClass (params Object[] args) {

        }
    }
}
